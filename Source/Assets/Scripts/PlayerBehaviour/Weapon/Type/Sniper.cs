using System.Collections;
using Photon.Pun;
using PlayerBehaviour.General;
using PlayerBehaviour.Interface;
using SCT;
using UI;
using UnityEngine;
using Utilities;
using VFX;

namespace PlayerBehaviour.Weapon.Type
{
	public class Sniper : WeaponBase
	{
		private float m_chargeTime = 0.0f;
		private bool m_charge = false;
		private Vector3 m_firePoint = new Vector3(0, 0, 0);
		private Vector3 m_fireDirection = new Vector3(0, 0, 0);
		private RaycastHit[] m_hits = new RaycastHit[10];
		private ParticleSystem m_hitEffect = null;
		private ParticleSystem m_chargeEffect = null;
		private LineRenderer m_lineRender = null;
		private LineAnimation m_lineAnimation = null;
		private float m_fireDistance = 0;
		private float m_currentTic = 0.0f;
		private float m_currentFadeTime = 0.0f;

		public override void Initialize(WeaponSocket weaponSocket, Weapon weapon, WeaponHud hud, AimOrigin aimOrigin,
			HitEvent hitEvent)
		{
			base.Initialize(weaponSocket, weapon, hud, aimOrigin, hitEvent);
			m_hitEffect = Instantiate(weapon.HitEffect);

			m_chargeEffect = Instantiate(weapon.ChargeEffect, WeaponObject.transform, false);
			m_chargeEffect.transform.localPosition = Weapon.MuzzleFlashOffset;

			//Get Components to Handle Effects
			var lineEffects = Instantiate(Weapon.TrailEffect, MuzzleFlash.transform, false);
			m_lineRender = lineEffects.GetComponent<LineRenderer>();
			m_lineRender.transform.localPosition = new Vector3(0, 0, 0);
			m_lineRender.positionCount = 2;
			m_lineRender.widthMultiplier = Weapon.RayThickness * 10;
			m_lineRender.enabled = false;

			m_lineAnimation = lineEffects.GetComponent<LineAnimation>();
		}

		/// <summary>Activate Charging.</summary>
		public override void Shoot()
		{
			if (WeaponCollision.IsColliding() || !HasAmmo) return;

			if (Time.time > CooldownCollapsed && !m_charge)
			{
				m_charge = true;
				PhotonView.RPC("ChargeEffect", RpcTarget.AllViaServer);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (m_chargeEffect != null)
			{
				m_chargeEffect.transform.localPosition = Weapon.MuzzleFlashOffset;
			}

			m_fireDistance = GetMaxDistance();
			Charging();
		}

		/// <summary> Using Raycast to get the distance between hit point and start point.</summary>
		/// <returns>Max Shoot Distance</returns>
		private float GetMaxDistance()
		{
			var ray = new Ray(AimOrigin.transform.position, MuzzleFlash.transform.forward);

			if (Physics.Raycast(ray
								, out var hit
								, Mathf.Infinity
								, 1 << 10
								, QueryTriggerInteraction.Ignore))
			{
				var distToGround = Vector3.Distance(MuzzleFlash.transform.position, hit.point);
				return distToGround;
			}

			return Weapon.MaxLength;
		}

		/// <summary>Counts Time to load the Weapon and shoots if Time is reached.</summary>
		private void Charging()
		{
			if (!m_charge) return;

			m_chargeTime += Time.deltaTime;


			//cache start point and start direction for fading
			// Fires if charge time is reached
			if (m_chargeTime >= Weapon.ChargeTime)
			{
				var startOffset = MuzzleFlash.transform.forward * (Weapon.RayThickness - 0.5f);

				m_firePoint = MuzzleFlash.transform.position - startOffset;
				m_fireDirection = WeaponSocket.transform.forward;

				Fire();

				m_chargeTime = 0.0f;
				m_charge = false;
			}
		}

		private void Fire()
		{
			PushBack();

			//Search for Target, deals damage to targets.
			var hits = SearchTargets();
			ApplyDamage(hits, Weapon.Damage);

			StartCoroutine(Fade());

			ReduceAmmo();
			PhotonView.RPC("ShootEffectRpc", RpcTarget.All, MuzzleFlash.transform.forward);
			CameraShake.StartShake(Weapon.ShakeProfile);

			CooldownCollapsed = Time.time + Weapon.FireRate;
		}

		/// <summary>
		/// Fires Sphere Cast with given Values to locate possibles targets
		/// </summary>
		private int SearchTargets()
		{
			var hits = Physics.SphereCastNonAlloc(m_firePoint
												, Weapon.RayThickness
												, m_fireDirection
												, m_hits
												, m_fireDistance
												, Weapon.HitMask);
#if UNITY_EDITOR
			for (var index = 0; index < hits; index++)
			{
				var hit = m_hits[index];

				if (hit.point != Vector3.zero)
				{
					//Using Debug Line to visualize hits
					Debug.DrawLine(m_firePoint, hit.point, Color.white, 2);
				}
			}
#endif

			return hits;
		}

		/// <summary>Deals Damage to Targets.</summary>
		private void ApplyDamage(int targetCount, float damage)
		{
			for (var i = 0; i < targetCount; i++)
			{
				var target = m_hits[i].transform.gameObject;

				if (!PlayerHelper.ValidateTarget(target)) continue;

				var damageableTarget = target.GetComponent<IDamageable>();
				var hitted = damageableTarget.ApplyDamage(damage);
				if (hitted)
				{
					HitEvent.Invoke();
					ScriptableTextDisplay.Instance.InitializeScriptableText(0, target.transform.position,
																			damage.ToString());
				}
			}
		}

		/// <summary>
		/// Applies damage tic wise  till the fade time reached. 
		/// </summary>
		/// <returns></returns>
		private IEnumerator Fade()
		{
			while (m_currentFadeTime <= Weapon.FadeOutTime)
			{
				m_currentTic += Time.deltaTime;
				m_currentFadeTime += Time.deltaTime;

				if (m_currentTic >= Weapon.TicRate)
				{
					var hits = SearchTargets();
					ApplyDamage(hits, Weapon.TicDamage);
					m_currentTic = 0;
				}

				yield return null;
			}

			m_currentFadeTime = m_currentTic = 0;
		}

		/// <summary>
		/// Displays effect if the weapon is fired.
		/// </summary>
		[PunRPC]
		private void ChargeEffect()
		{
			OnFirstShot();
			m_chargeEffect.gameObject.SetActive(true);
			m_chargeEffect.Play(true);
			WeaponAudio.clip = Weapon.ChargeAudio;
			WeaponAudio.Play();
		}

		/// <summary>
		///	Creates a effect based on direction. 
		/// </summary>
		/// <param name="dir">shoot direction</param>
		[PunRPC]
		protected override void ShootEffectRpc(Vector3 dir)
		{
			m_chargeEffect.gameObject.SetActive(false);
			WeaponAudio.Stop();

			var startPoint = MuzzleFlash.transform.position;
			var endPoint = MuzzleFlash.transform.position + dir * m_fireDistance;
			Recoil();
			PlayMuzzleFlash();
			PlayAudio();
			m_lineRender.SetPosition(0, startPoint);
			m_lineRender.SetPosition(1, endPoint);

			m_hitEffect.transform.position = endPoint;
			m_hitEffect.Play(true);

			m_lineAnimation.Play();
		}

		public override void DestroyWeapon()
		{
			if (m_hitEffect != null)
			{
				Destroy(m_hitEffect.gameObject);
			}

			if (m_chargeEffect != null)
			{
				Destroy(m_chargeEffect.gameObject);
			}

			base.DestroyWeapon();
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (MuzzleFlash.transform != null)
			{
				GizmoExtension.DrawCapsule(MuzzleFlash.transform, Weapon.RayThickness, m_fireDistance);
			}
		}
#endif
	}
}