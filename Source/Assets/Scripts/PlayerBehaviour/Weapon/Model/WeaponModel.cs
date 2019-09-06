using System;
using NaughtyAttributes;
using Network.Extensions;
using Photon.Pun;
using PlayerBehaviour.Interface;
using PlayerBehaviour.Weapon.Type;
using UI;
using UnityEngine;

namespace PlayerBehaviour.Weapon.Model
{
	public class WeaponModel : MonoBehaviour, IPickable, IUpgradeable
	{
		#region Events

		public Action<string> OnWeaponChanged;
		public Action OnUpgradeWeapon;

		#endregion

		public TriggerType TriggerType
		{
			get { return m_currentWeaponMechanic.TriggerType; }
		}

		[SerializeField] private HitEvent HitEvent = null;
		[SerializeField] private WeaponHud WeaponHud = null;
		[SerializeField] private AimOrigin AimOrigin = null;
		[SerializeField] private WeaponDatabase WeaponDatabase = null;
		[SerializeField] private WeaponSocket WeaponSocket = null;
		[SerializeField] private Weapon DefaultWeapon = null;
		[SerializeField, ReadOnly] private Weapon CurrentWeapon = null;

		private WeaponBase m_currentWeaponMechanic = null;
		private PhotonView m_photonView = null;

		private void Start()
		{
			m_photonView = GetComponent<PhotonView>();
			SetDefaultWeapon();
		}

		private void SetDefaultWeapon()
		{
			var targetWeapon = DefaultWeapon;

			//King gets instant upgrade
			if (m_photonView.Owner.IsKing())
			{
				targetWeapon = DefaultWeapon.Upgrade;
			}

			SetWeapon(targetWeapon);
		}

		private void SetWeapon(Weapon weapon)
		{
			CurrentWeapon = weapon;

			if (m_currentWeaponMechanic != null)
			{
				m_currentWeaponMechanic.DestroyWeapon();
			}

			m_currentWeaponMechanic = AddMechanic(CurrentWeapon.WeaponType);
			m_currentWeaponMechanic.Initialize(WeaponSocket, weapon, WeaponHud, AimOrigin, HitEvent);
		}

		internal void Shoot()
		{
			if (UiSelection.Instance.HasFocus) return;
			m_currentWeaponMechanic.Shoot();
		}

		internal void StoppedShooting()
		{
			m_currentWeaponMechanic.StopShooting();
		}

		internal void Reload()
		{
			m_currentWeaponMechanic.Reload();
		}

		public void PickUp(Weapon weaponToPickup)
		{
			var targetWeapon = weaponToPickup;

			if (m_photonView.Owner.IsKing())
			{
				targetWeapon = weaponToPickup.Upgrade;
			}

			ChangeWeaponTo(targetWeapon);
		}

		public void Upgrade()
		{
			ChangeWeaponTo(CurrentWeapon.Upgrade);
		}

		private void ChangeWeaponTo(Weapon target)
		{
			if (target == null) return;

			var idx = WeaponDatabase.GetIndex(target);
			m_photonView.RPC("ChangeWeaponRPC", RpcTarget.AllViaServer, idx, true);
			OnWeaponChanged?.Invoke(target.WeaponName);
		}

		[PunRPC]
		private void ChangeWeaponRPC(int index, bool isUpgrade)
		{
			if (isUpgrade)
			{
				OnUpgradeWeapon?.Invoke();
			}


			var newWeapon = WeaponDatabase.GetWeapon(index);
			SetWeapon(newWeapon);
		}

		/// <summary>
		/// Add Component based on given Type.
		/// </summary>
		private WeaponBase AddMechanic(WeaponType type)
		{
			WeaponBase weaponBase = null;
			switch (type)
			{
				case WeaponType.Pistol:
					weaponBase = gameObject.AddComponent<Pistol>();
					break;
				case WeaponType.AssaultRifle:
					weaponBase = gameObject.AddComponent<AssaultRifle>();
					break;
				case WeaponType.ShotGun:
					weaponBase = gameObject.AddComponent<ShotGun>();
					break;
				case WeaponType.Laser:
					weaponBase = gameObject.AddComponent<Laser>();
					break;
				case WeaponType.Sniper:
					weaponBase = gameObject.AddComponent<Sniper>();
					break;
			}

			return weaponBase;
		}
	}
}