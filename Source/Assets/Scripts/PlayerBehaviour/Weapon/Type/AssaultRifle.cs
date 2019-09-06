using Photon.Pun;
using PlayerBehaviour.General;
using PlayerBehaviour.Interface;
using SCT;
using UI;
using UnityEngine;

namespace PlayerBehaviour.Weapon.Type
{
	public class AssaultRifle : WeaponBase, IPunObservable
	{
		private ParticleSystem m_hitEffect = null;
		private TrailRenderer m_trailEffect = null;
		private RaycastHit m_raycastHit = new RaycastHit();

		public override void Initialize(WeaponSocket weaponSocket, Weapon weapon, WeaponHud hud, AimOrigin aimOrigin,
			HitEvent hitEvent)
		{
			base.Initialize(weaponSocket, weapon, hud, aimOrigin, hitEvent);
			m_hitEffect = Instantiate(weapon.HitEffect);
			PhotonView.ObservedComponents.Add(this);

			m_trailEffect = Instantiate(Weapon.TrailEffect, MuzzleFlash.transform, false).GetComponent<TrailRenderer>();
			m_trailEffect.transform.localPosition = new Vector3(0, 0, 0);
		}

		public override void Shoot()
		{
			if (WeaponCollision.IsColliding() || IsReloading)
			{
				IsShooting = false;
			}
			else
			{
				IsShooting = true;
			}
		}

		protected override void Update()
		{
			base.Update();
			if (!IsShooting) return;

			HandelShooting();
		}

		private void HandelShooting()
		{
			if (Time.time <= CooldownCollapsed) return;


			var ray = new Ray(AimOrigin.transform.position, WeaponSocket.transform.forward);
			if (Physics.Raycast(ray, out m_raycastHit, Mathf.Infinity, Weapon.HitMask,
								QueryTriggerInteraction.Ignore))
			{
				if (PhotonView.IsMine)
				{
					var target = m_raycastHit.transform.GetComponent<IDamageable>();

					if (PlayerHelper.ValidateTarget(m_raycastHit.transform.gameObject))
					{
						var hitted = target.ApplyDamage(Weapon.Damage);
						if (hitted)
						{
							HitEvent.Invoke();
							ScriptableTextDisplay.Instance.InitializeStackingScriptableText(1, m_raycastHit.point,
																							Weapon.Damage.ToString(),
																							m_raycastHit.transform
																								.gameObject
																								.name);
						}
					}
				}
			}

			if (PhotonView.IsMine)
			{
				PushBack();
				ReduceAmmo();
				CameraShake.StartShake(Weapon.ShakeProfile);
			}

			var hitPoint = m_raycastHit.point == new Vector3(0, 0, 0)
				? MuzzleFlash.transform.position + WeaponSocket.transform.forward * 200
				: m_raycastHit.point;

			ShootEffect(hitPoint);
			CooldownCollapsed = Time.time + Weapon.FireRate;
		}

		protected override void ShootEffectRpc(Vector3 vec3) { }

		private void ShootEffect(Vector3 hitPoint)
		{
			OnFirstShot();
			Recoil();
			PlayMuzzleFlash();
			PlayAudio();
			m_hitEffect.transform.position = hitPoint;
			m_hitEffect.Play(true);

			m_trailEffect.AddPositions(new[] {MuzzleFlash.transform.position, hitPoint});
		}

		public override void StopShooting()
		{
			m_trailEffect.Clear();
			IsShooting = false;
		}

		public override void DestroyWeapon()
		{
			if (m_hitEffect != null)
			{
				Destroy(m_hitEffect.gameObject);
			}

			if (m_trailEffect != null)
			{
				Destroy(m_trailEffect.gameObject);
			}

			base.DestroyWeapon();
		}

		/// <summary>
		/// Photon Callback. Used to only sync if client is shooting or not
		/// </summary>
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(IsShooting);
			}
			else
			{
				IsShooting = (bool) stream.ReceiveNext();
			}
		}

		private void OnDestroy()
		{
			PhotonView.ObservedComponents.Remove(this);
		}
	}
}