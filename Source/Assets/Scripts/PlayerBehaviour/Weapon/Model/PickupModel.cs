using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Network.Extensions;
using Photon.Pun;
using PlayerBehaviour.Interface;
using UnityEngine;
using Random = System.Random;

namespace PlayerBehaviour.Weapon.Model
{
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(BoxCollider))]
	public class PickupModel : MonoBehaviour
	{
		#region Events

		public Action TriggerEntered;
		public Action TriggerExited;
		public Action PickedWeapon;
		public Action<bool> ChangeState;

		#endregion

		/// <summary>
		/// Used for random weapon spawn, based on probability.
		/// </summary>
		[System.Serializable]
		public class WeightedSpawn
		{
			[HideInInspector] public string Name = "";
			public Weapon Weapon = null;
			public int Weight = 0;
		}

		public bool Upgrade = false;
		[SerializeField, DisableIf("Upgrade")] private List<WeightedSpawn> WeightedSpawns = new List<WeightedSpawn>();
		[SerializeField] private GameObject UpgradePreview = null;
		[SerializeField] private Transform Socket = null;
		[SerializeField] private BoxCollider Collider = null;

		private float m_currentRespawnTime = 0.0f;
		private Weapon m_weapon = null;
		private IPickable m_target = null;
		private IUpgradeable m_upgradeableTarget = null;
		private Transform m_weaponOnSocket = null;
		private PhotonView m_photonView = null;
		private Random m_rnd;

#if UNITY_EDITOR
		/// <summary>Just for the Game Designer.</summary>
		private void OnValidate()
		{
			foreach (var item in WeightedSpawns)
			{
				if (item.Weapon != null)
				{
					item.Name = item.Weapon.name;
				}
				else
				{
					item.Name = "No Weapon";
				}
			}
		}
#endif

		private void Start()
		{
			m_photonView = GetComponent<PhotonView>();
			Collider = GetComponent<BoxCollider>();

			//to make sure each client uses the same seed and generates the same random numbers
			var seed = PhotonNetwork.MasterClient.NickName.GetHashCode()
						+ (int) transform.position.x
						+ PhotonNetwork.CurrentRoom.GetHashCode();

			m_rnd = new Random(seed);
			GetSpawnTime();

			Initialize();
		}

		/// <summary>
		/// Create initial state.
		/// </summary>
		private void Initialize()
		{
			if (Upgrade)
			{
				UpgradePreview.SetActive(true);
			}
			else
			{
				UpgradePreview.SetActive(false);
				ChangeStateRPC(true);
			}

			//PickUp is ready
			ChangeState?.Invoke(true);
		}

		/// <summary>
		/// Read Spawn Time from Properties.
		/// </summary>
		private void GetSpawnTime()
		{
			m_currentRespawnTime = PhotonNetwork.CurrentRoom.GetWeaponSpawnTime();
		}

		/// <summary>
		/// Called from Controller, if target available proceed.
		/// </summary>
		internal void PickUp()
		{
			if (m_target == null || m_upgradeableTarget == null) return;

			if (Upgrade)
			{
				m_upgradeableTarget?.Upgrade();
			}
			else
			{
				m_target?.PickUp(m_weapon);
			}

			m_target = null;
			m_upgradeableTarget = null;

			m_photonView.RPC("ChangeStateRPC", RpcTarget.All, false);
			PickedWeapon?.Invoke();
		}

		/// <summary>
		/// Creating Weapon Model as Preview or using Upgrade model.
		/// </summary>
		private void CreatePreview()
		{
			UpgradePreview.SetActive(Upgrade);
			if (Upgrade) return;

			m_weapon = GetRandomWeapon();
			m_weaponOnSocket = Instantiate(m_weapon.Model.gameObject, Socket, false).transform;
			m_weaponOnSocket.transform.localPosition = new Vector3(0, 0, 0);
		}

		/// <summary>
		/// Handling PickUp state.
		/// </summary>
		/// <param name="enabled"></param>
		[PunRPC]
		private void ChangeStateRPC(bool enabled)
		{
			if (Upgrade)
			{
				UpgradePreview.SetActive(enabled);

				if (enabled)
				{
					m_upgradeableTarget = null;
				}
			}
			else
			{
				if (enabled)
				{
					CreatePreview();
				}
				else
				{
					m_target = null;
					if (m_weaponOnSocket != null)
					{
						Destroy(m_weaponOnSocket.gameObject);
					}
				}
			}

			if (PhotonNetwork.IsMasterClient && enabled == false)
			{
				StartCoroutine(Respawn());
			}

			ChangeState?.Invoke(enabled);
			Collider.enabled = enabled;
		}

		private IEnumerator Respawn()
		{
			yield return new WaitForSeconds(m_currentRespawnTime);
			m_photonView.RPC("ChangeStateRPC", RpcTarget.All, true);
		}

		#region Collision

		private void OnTriggerEnter(Collider other)
		{
			var targetView = other.GetComponent<PhotonView>();
			if (targetView == null || !targetView.IsMine) return;

			m_target = other.GetComponent<IPickable>();
			m_upgradeableTarget = other.GetComponent<IUpgradeable>();

			TriggerEntered?.Invoke();
		}

		private void OnTriggerExit(Collider other)
		{
			m_target = null;
			m_upgradeableTarget = null;
			TriggerExited?.Invoke();
		}

		#endregion

		#region Probability

		private Weapon GetRandomWeapon()
		{
			var totalMass = GetTotalMass();
			var randVal = m_rnd.Next(0, totalMass + 1);
			var total = 0;
			foreach (var t in WeightedSpawns)
			{
				total += t.Weight;
				if (total > randVal)
				{
					return t.Weapon;
				}
			}

			return WeightedSpawns[0].Weapon;
		}

		private int GetTotalMass()
		{
			var totalMass = 0;
			foreach (var t in WeightedSpawns)
			{
				totalMass += t.Weight;
			}

			return totalMass;
		}

		#endregion

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (Collider)
			{
				Gizmos.DrawWireCube(transform.position, Collider.size);
			}
		}
#endif
	}
}