using PlayerBehaviour.Model;
using PlayerBehaviour.Weapon;
using PlayerBehaviour.Weapon.Model;
using UI;
using UnityEngine;

namespace PlayerBehaviour.Controller
{
	/// <summary>
	/// Receive Input and progress it to Model.
	/// </summary>
	[RequireComponent(typeof(PlayerMovementModel))]
	[RequireComponent(typeof(PlayerBootsModel))]
	[RequireComponent(typeof(WeaponModel))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private KeyCode ShootKey = KeyCode.Mouse0;
		[SerializeField] private KeyCode ReloadKey = KeyCode.R;

		private PlayerMovementModel m_playerMovementModel;
		private PlayerBootsModel m_playerBootsModel;
		private WeaponModel m_weaponModel;

		private void Start()
		{
			m_playerMovementModel = GetComponent<PlayerMovementModel>();
			m_playerBootsModel = GetComponent<PlayerBootsModel>();
			m_weaponModel = GetComponent<WeaponModel>();
		}

		private void Update()
		{
			PlayerInput();
		}

		/// <summary>
		/// Wraps all Input.
		/// </summary>
		private void PlayerInput()
		{
			if (!m_playerMovementModel.PhotonView.IsMine || UiSelection.Instance.HasFocus) return;

			Movement();
			UseBoots();
			Shoot();
			Reload();

			m_playerMovementModel.Aim();
			m_playerMovementModel.Facing();
			m_playerMovementModel.HandleShoulderRotation();
		}

		private void Movement()
		{
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");

			m_playerMovementModel.PlayerInput(horizontal, vertical);
		}

		private void UseBoots()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				m_playerBootsModel.Use();
			}
		}

		private void Shoot()
		{
			switch (m_weaponModel.TriggerType)
			{
				case TriggerType.OnTriggerPressed:
					if (Input.GetKeyDown(ShootKey))
					{
						m_weaponModel.Shoot();
					}

					break;
				case TriggerType.OnTriggerHold:
					if (Input.GetKey(ShootKey))
					{
						m_weaponModel.Shoot();
					}

					break;
			}

			if (Input.GetKeyUp(ShootKey))
			{
				m_weaponModel.StoppedShooting();
			}
		}

		private void Reload()
		{
			if (Input.GetKey(ReloadKey))
			{
				m_weaponModel.Reload();
			}
		}
	}
}