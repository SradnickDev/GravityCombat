using PlayerBehaviour.Weapon.Model;
using UnityEngine;

namespace PlayerBehaviour.Weapon.View
{
	[RequireComponent(typeof(PickupModel))]
	public class PickUpView : MonoBehaviour
	{
		[SerializeField] private GameObject Display = null;
		[SerializeField] private ParticleSystem Active = null;
		[SerializeField] private ParticleSystem Inactive = null;
		[SerializeField] private AudioClip PickUpClip = null;
		[SerializeField] private AudioClip UpgradeClip = null;
		[SerializeField] private AudioSource PickUpSource = null;

		private PickupModel m_pickupModel = null;

		private void Start()
		{
			if (Display == null)
			{
				Debug.LogWarning("No Display Assigned for Pick up View!");
			}

			m_pickupModel = GetComponent<PickupModel>();
			m_pickupModel.TriggerExited += OnTriggerExited;
			m_pickupModel.TriggerEntered += OnTriggerEntered;
			m_pickupModel.PickedWeapon += OnPickedUpWeapon;
			m_pickupModel.ChangeState += OnStateChanged;
		}

		#region Event Reactions

		private void OnTriggerEntered()
		{
			Display.SetActive(true);
		}

		private void OnTriggerExited()
		{
			Display.SetActive(false);
		}

		private void OnPickedUpWeapon()
		{
			if (PickUpSource != null)
			{
				if (m_pickupModel.Upgrade == false)
				{
					PickUpSource.clip = PickUpClip;
					PickUpSource.Play();
				}
				else
				{
					PickUpSource.clip = UpgradeClip;
					PickUpSource.Play();
				}
			}

			Display.SetActive(false);
		}

		private void OnStateChanged(bool active)
		{
			if (active)
			{
				Active.Play(true);
				Inactive.Stop(true);
			}
			else
			{
				Active.Stop(true);
				Inactive.Play(true);
			}
		}

		#endregion
	}
}