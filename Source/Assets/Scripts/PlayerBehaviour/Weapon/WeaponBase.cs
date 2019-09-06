using System.Collections;
using Cam;
using Photon.Pun;
using PlayerBehaviour.General;
using PlayerBehaviour.Model;
using UI;
using UnityEngine;

namespace PlayerBehaviour.Weapon
{
	/// <summary>
	/// Base Class for all Weapons, to create a new Type use this to inherit from, will provide basic mechanics.
	/// </summary>
	public abstract class WeaponBase : MonoBehaviour
	{
		public TriggerType TriggerType
		{
			get { return Weapon.TriggerType; }
		}

		protected Weapon Weapon = null;
		protected float CooldownCollapsed = 0;
		protected bool IsReloading = false;
		protected ParticleSystem MuzzleFlash = null;
		protected PhotonView PhotonView;
		protected WeaponSocket WeaponSocket;
		protected CameraShake CameraShake = null;
		protected bool IsShooting = false;
		protected AudioSource WeaponAudio = null;
		protected WeaponCollision WeaponCollision = null;
		protected AimOrigin AimOrigin = null;
		protected GameObject WeaponObject = null;
		protected HitEvent HitEvent = null;

		protected bool HasAmmo
		{
			get { return m_currentAmmo > 0; }
		}

		private Shield m_shield = null;
		private PlayerMovementModel m_playerMovementModel = null;
		private WeaponHud m_weaponHud = null;
		private int m_currentAmmo = 0;
		private Rigidbody m_rigidbody = null;
		private Vector3 m_recoilOffset = new Vector3(0, 0, 0);
		private Vector3 m_dampRef = new Vector3(0, 0, 0);
		private bool m_firstShot = false;

		public virtual void Initialize(WeaponSocket weaponSocket, Weapon weapon, WeaponHud hud, AimOrigin aimOrigin,
			HitEvent hitEvent)
		{
			PhotonView = GetComponent<PhotonView>();
			m_rigidbody = GetComponent<Rigidbody>();
			WeaponAudio = GetComponent<AudioSource>();
			m_shield = GetComponent<Shield>();
			m_playerMovementModel = GetComponent<PlayerMovementModel>();
			HitEvent = hitEvent;
			m_weaponHud = hud;
			WeaponSocket = weaponSocket;
			Weapon = weapon;
			AimOrigin = aimOrigin;
			CameraShake = CameraShake.Instance;

			CreateWeapon();
			CreateMuzzleFlash();

			WeaponCollision = WeaponObject.GetComponent<WeaponCollision>();

			m_currentAmmo = Weapon.AmmoClip;

			m_weaponHud.SetAmmoAmount(m_currentAmmo, Weapon.AmmoClip);
		}

		/// <summary>Create Weapon GameObject if available.</summary>
		private void CreateWeapon()
		{
			if (Weapon.Model == null) return;

			WeaponObject = Instantiate(Weapon.Model, WeaponSocket.transform, false);
			WeaponObject.transform.localPosition = Weapon.PositionOffset;
		}

		/// <summary>Creates MuzzleFlash if not assigned default Particle System will be created.</summary>
		private void CreateMuzzleFlash()
		{
			if (WeaponObject == null) return;

			if (Weapon.MuzzleFlash != null)
			{
				MuzzleFlash = Instantiate(Weapon.MuzzleFlash, WeaponObject.transform, false);
				MuzzleFlash.transform.localPosition = Weapon.MuzzleFlashOffset;
			}
			else
			{
				MuzzleFlash = new GameObject("replacement").AddComponent<ParticleSystem>();
				MuzzleFlash.transform.SetParent(WeaponObject.transform, false);
				MuzzleFlash.transform.localPosition = Weapon.MuzzleFlashOffset;
				Debug.LogError(Weapon + " has no MuzzleFlash assigned!");
			}
		}

		protected void OnFirstShot()
		{
			if (!m_firstShot)
			{
				m_shield.DisableShield();
				m_firstShot = true;
			}
		}

		/// <summary>Should be used to creat an Individual Shooting mechanic.</summary>
		public abstract void Shoot();

		public virtual void StopShooting()
		{
			IsShooting = false;
		}

		/// <summary>Decrease Current Ammo, reload if empty.</summary>
		protected void ReduceAmmo()
		{
			//unlimited
			if (Weapon.AmmoClip == -1) return;

			m_currentAmmo = Mathf.Clamp(m_currentAmmo -= 1, 0, Weapon.AmmoClip);
			m_weaponHud.SetAmmoAmount(m_currentAmmo, Weapon.AmmoClip);

			//Reload
			if (m_currentAmmo <= 0)
			{
				Reload();
			}
		}

		/// <summary>Start IEnumerator -> DoReload.</summary>
		internal void Reload()
		{
			if (IsReloading) return;
			StopShooting();
			IsShooting = false;
			IsReloading = true;
			StartCoroutine(DoReload());
			m_weaponHud.Reload(Weapon.ReloadTime);
		}

		/// <summary>
		/// Performs a delay and resets the Ammo to max.
		/// </summary>
		/// <returns></returns>
		private IEnumerator DoReload()
		{
			yield return new WaitForSeconds(Weapon.ReloadTime);
			m_currentAmmo = Weapon.AmmoClip;
			m_weaponHud.SetAmmoAmount(m_currentAmmo, Weapon.AmmoClip);
			IsReloading = false;
		}

		/// <summary>
		/// Used for effects.
		/// </summary>
		[PunRPC]
		protected abstract void ShootEffectRpc(Vector3 vec3);

		/// <summary>Applies a random recoil between min an max.</summary>
		protected void Recoil()
		{
			m_recoilOffset.z = Random.Range(-Weapon.Recoil.x, -Weapon.Recoil.y);
		}

		/// <summary>Applies a Impulse Force to the Rigidbody.</summary>
		protected void PushBack()
		{
			if (Mathf.Approximately(Weapon.PushBack, 0.0f) || !m_playerMovementModel.IsGrounded) return;


			m_rigidbody.AddForce(-WeaponSocket.transform.forward * Weapon.PushBack, ForceMode.Impulse);
		}

		/// <summary> Play usually a Shoot Sound</summary>
		protected virtual void PlayAudio()
		{
			if (Weapon.ShootAudio != null)
			{
				WeaponAudio.PlayOneShot(Weapon.ShootAudio);
			}
			else
			{
				Debug.LogError(Weapon.name + " No Shoot audio assigned!");
			}
		}

		/// <summary>Play Particle Effect.</summary>
		protected virtual void PlayMuzzleFlash()
		{
			if (MuzzleFlash != null)
			{
				MuzzleFlash.Play(true);
			}
			else
			{
				Debug.LogError(Weapon.name + " No MuzzleFlash assigned!");
			}
		}

		protected virtual void Update()
		{
			m_recoilOffset =
				Vector3.SmoothDamp(m_recoilOffset, Vector3.zero, ref m_dampRef, Weapon.RecoilResetDuration);
			WeaponSocket.transform.localPosition = Weapon.PositionOffset + m_recoilOffset;

#if UNITY_EDITOR
			if (MuzzleFlash != null)
			{
				MuzzleFlash.transform.localPosition = Weapon.MuzzleFlashOffset;
			}

			WeaponObject.transform.localPosition = Weapon.PositionOffset;
#endif
		}

		/// <summary>Destroy relevant GameObjects and Components from Weapon.</summary>
		public virtual void DestroyWeapon()
		{
			m_weaponHud.Reset();

			if (MuzzleFlash != null)
			{
				Destroy(MuzzleFlash.gameObject);
			}

			if (WeaponObject != null)
			{
				Destroy(WeaponObject);
			}

			if (WeaponAudio != null)
			{
				WeaponAudio.Stop();
			}

			Destroy(this);
		}
	}
}