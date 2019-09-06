using PlayerBehaviour.Weapon.Model;
using SCT;
using UnityEngine;

namespace PlayerBehaviour.Weapon.View
{
	/// <summary>
	/// Handling upgrade effect.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(WeaponModel))]
	public class WeaponView : MonoBehaviour
	{
		[SerializeField] private ParticleSystem UpgradeEffect = null;

		private WeaponModel m_weaponModel = null;

		private void Start()
		{
			m_weaponModel = GetComponent<WeaponModel>();

			m_weaponModel.OnUpgradeWeapon += OnUpgradeWeapon;
			m_weaponModel.OnWeaponChanged += OnWeaponChanged;
		}

		private void OnWeaponChanged(string newWeapon)
		{
			ScriptableTextDisplay.Instance.InitializeScriptableText(10, transform.position + transform.up * 2,
																	newWeapon);
		}

		private void OnDisable()
		{
			m_weaponModel.OnUpgradeWeapon -= OnUpgradeWeapon;
			m_weaponModel.OnWeaponChanged -= OnWeaponChanged;
		}

		private void OnUpgradeWeapon()
		{
			if (UpgradeEffect == null) return;

			if (UpgradeEffect.isPlaying)
			{
				UpgradeEffect.Stop(true);
			}

			UpgradeEffect.Play(true);
		}
	}
}