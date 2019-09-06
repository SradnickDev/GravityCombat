using Photon.Pun;
using PlayerBehaviour.Weapon.Model;
using UI;
using UnityEngine;

namespace PlayerBehaviour.Weapon
{
	[RequireComponent(typeof(PhotonView))]
	public class PickUpController : MonoBehaviour
	{
		[SerializeField] private KeyCode PickUpKey = KeyCode.E;
		private PickupModel m_pickupModel = null;

		private void Start()
		{
			m_pickupModel = GetComponent<PickupModel>();
		}

		private void Update()
		{
			PlayerInput();
		}

		private void PlayerInput()
		{
			if (UiSelection.Instance.HasFocus) return;

			if (Input.GetKeyDown(PickUpKey))
			{
				m_pickupModel.PickUp();
			}
		}
	}
}