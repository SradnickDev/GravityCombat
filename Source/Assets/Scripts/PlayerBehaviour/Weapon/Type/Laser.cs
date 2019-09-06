using System.Collections.Generic;
using Photon.Pun;
using PlayerBehaviour.General;
using PlayerBehaviour.Interface;
using SCT;
using UI;
using UnityEngine;

namespace PlayerBehaviour.Weapon.Type
{
	public class Laser : WeaponBase, IPunObservable
	{
		private ParticleSystem[] m_hitEffects = null;
		private LineRenderer m_trailEffect = null;

		private int m_rayNums = 0;
		private Ray[] m_rays;
		private List<Vector3> m_hits = new List<Vector3>();
		private GameObject m_target = null;
		private float m_currentLength = 0.0f;

		/// <summary>
		/// Using Base Init for default Setup.
		/// Creating needed Setup for Laser.
		/// </summary>
		/// <param name="weaponSocket">Place for Weapon to create</param>
		/// <param name="weapon">What Weapon</param>
		/// <param name="hud">Weapon View for Visuals</param>
		/// <param name="aimOrigin"></param>
		/// <param name="hitEvent"></param>
		public override void Initialize(WeaponSocket weaponSocket, Weapon weapon, WeaponHud hud, AimOrigin aimOrigin,
			HitEvent hitEvent)
		{
			base.Initialize(weaponSocket, weapon, hud, aimOrigin, hitEvent);

			PhotonView.ObservedComponents.Add(this);

			// 1 default + reflect count
			m_rayNums = 1 + Weapon.ReflectCount;

			m_rays = new Ray[m_rayNums + 1];

			//max x reflections = x hits + 1 end point 
			m_hitEffects = new ParticleSystem[Weapon.ReflectCount + 1];

			//each reflection is a hit + 1 end point
			for (var i = 0; i < Weapon.ReflectCount + 1; i++)
			{
				m_hitEffects[i] = Instantiate(weapon.HitEffect);
			}

			m_trailEffect = Instantiate(Weapon.TrailEffect, MuzzleFlash.transform, false).GetComponent<LineRenderer>();
			m_trailEffect.transform.localPosition = new Vector3(0, 0, 0);

			m_currentLength = Weapon.MaxLength;
		}

		/// <summary>
		/// Checks if Weapon is able to Shoot, applies Damage, Reduce ammo, Camera Shake and Push back.
		/// </summary>
		public override void Shoot()
		{
			if (WeaponCollision.IsColliding() || IsReloading)
			{
				IsShooting = false;
				return;
			}

			IsShooting = true;

			if (Time.time <= CooldownCollapsed) return;


			if (m_target != null && PlayerHelper.ValidateTarget(m_target))
			{
				var hitted = m_target.GetComponent<IDamageable>().ApplyDamage(Weapon.Damage);
				if (hitted)
				{
					HitEvent.Invoke();
					ScriptableTextDisplay.Instance.InitializeStackingScriptableText(5, m_target.transform.position,
																					Weapon.Damage.ToString(),
																					m_target.name);
				}
			}


			//PushBack
			PushBack();

			//Ammo
			ReduceAmmo();

			//Camera Shake
			CameraShake.StartShake(Weapon.ShakeProfile);

			CooldownCollapsed = Time.time + Weapon.FireRate;
		}

		protected override void Update()
		{
			base.Update();
			HandleRaycast();
		}

		/// <summary>
		/// Creating a Raycast, reflect hit point if non player is hitted.
		/// </summary>
		private void HandleRaycast()
		{
			if (IsShooting)
			{
				OnFirstShot();
				m_hits = new List<Vector3>();
				m_target = null;

				m_rays[0] = new Ray(AimOrigin.transform.position, WeaponSocket.transform.forward);
				m_hits.Add(MuzzleFlash.transform.position);

				var hitCount = 0;
				m_currentLength = Weapon.MaxLength;

				for (var i = 0; i < m_rayNums; i++)
				{
					if (Physics.Raycast(m_rays[i], out var hit, m_currentLength, Weapon.HitMask,
										QueryTriggerInteraction.Ignore))
					{
						var target = hit.transform.GetComponent<IDamageable>();
						m_currentLength -= Vector3.Distance(m_rays[i].origin, hit.point);


						m_hits.Add(hit.point);
						hitCount++;
						if (target != null)
						{
							var newTarget = hit.transform.gameObject;
							m_target = newTarget;
							break;
						}

						var reflect = Vector3.Reflect(m_rays[i].direction, hit.normal);
						m_rays[i + 1] = new Ray(hit.point, reflect);
					}
					else
					{
						m_hits.Add(m_rays[hitCount].origin + m_rays[hitCount].direction * m_currentLength);
						break;
					}
				}


				Effects(m_hits);
			}
			else
			{
				StopShooting();
			}
		}

		public override void StopShooting()
		{
			base.StopShooting();
			ResetLaser();
			m_currentLength = Weapon.MaxLength;
		}

		private void ResetLaser()
		{
			m_hits = new List<Vector3>();
			m_target = null;
			Effects(m_hits);
			if (MuzzleFlash.isPlaying)
			{
				MuzzleFlash.Stop(true);
			}

			WeaponAudio.Stop();
			m_rays = new Ray[m_rayNums + 1];
		}

		protected override void ShootEffectRpc(Vector3 vec3) { }

		/// <summary>
		/// Creating a Laser Effect out of cached hit points.
		/// </summary>
		/// <param name="hitPoints"></param>
		private void Effects(List<Vector3> hitPoints)
		{
			m_trailEffect.positionCount = hitPoints.Count;

			for (var i = 0; i < hitPoints.Count; i++)
			{
				if (i > 0)
				{
					m_hitEffects[i - 1].transform.position = hitPoints[i];
					m_hitEffects[i - 1].Play(true);
				}

				m_trailEffect.SetPosition(i, new Vector3(m_hits[i].x, m_hits[i].y, MuzzleFlash.transform.position.z));
			}

			if (!MuzzleFlash.isPlaying)
			{
				MuzzleFlash.Play(true);
			}

			PlayAudio();
		}

		protected override void PlayAudio()
		{
			if (Weapon.ShootAudio != null)
			{
				if (WeaponAudio.isPlaying) return;

				WeaponAudio.clip = Weapon.ShootAudio;
				WeaponAudio.Play();
			}
			else
			{
				Debug.LogError(Weapon.name + " No Shoot audio assigned!");
			}
		}

		public override void DestroyWeapon()
		{
			foreach (var effect in m_hitEffects)
			{
				Destroy(effect.gameObject);
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
		/// <param name="stream"></param>
		/// <param name="info"></param>
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