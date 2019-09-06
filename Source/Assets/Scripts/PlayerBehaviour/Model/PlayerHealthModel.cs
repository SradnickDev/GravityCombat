using System;
using System.Collections.Generic;
using Network.Extensions;
using Photon.Pun;
using PlayerBehaviour.General;
using PlayerBehaviour.Interface;
using UnityEngine;

namespace PlayerBehaviour.Model
{
	public class PlayerHealthModel : MonoBehaviour, IPunObservable, IDamageable
	{
		private const int KillPoints = 6;
		private const int DamageForAssist = 30;
		private const int AssistPoints = 2;
		private const int DeathPointLose = 1;

		#region Events

		public event Action OnLocalHit;
		public event Action<float, float> OnChangeHealthEvent;
		public event Action<float> OnReceivedDamage;
		public event Action<Photon.Realtime.Player> OnPlayerDeath;

		#endregion

		[SerializeField] private float MaxHealth = 0;
		[SerializeField] private float TimeTillRegenerate = 5;
		[SerializeField] private float HealthPerSeconds = 5;

		public Team CurrentTeam { get; set; }
		public Camera Camera = null;

		private PhotonView m_photonView = null;
		private float m_currentHealth = 0;
		private Photon.Realtime.Player m_lastHit = null;
		private float m_regStartTime = 0.0f;
		private bool m_isRegenerating = false;
		private Shield m_shield = null;

		public bool IsRegenerating
		{
			get { return m_isRegenerating; }
		}

		private Dictionary<Photon.Realtime.Player, float> m_receivedHits =
			new Dictionary<Photon.Realtime.Player, float>();

		private void Start()
		{
			if (m_photonView == null)
			{
				m_photonView = GetComponent<PhotonView>();
			}

			if (m_photonView.Owner.IsKing())
			{
				MaxHealth = PhotonNetwork.CurrentRoom.GetKingHealth();
			}

			m_shield = GetComponent<Shield>();

			//Init, local on every Client
			m_currentHealth = MaxHealth;
			if (m_photonView.IsMine)
			{
				m_photonView.Owner.SetAlive(true);
			}

			OnChangeHealthEvent?.Invoke(m_currentHealth, MaxHealth);

			//non owner also needs a ref to the camera, owner gets one through playersetup.cs
			Camera = Camera.main;
		}

		/// <summary>Add Damage to Target.</summary>
		/// <param name="damage">Value</param>
		public bool ApplyDamage(float damage)
		{
			if (m_shield.IsActive)
			{
				return false;
			}

			m_photonView.RPC("ReceiveDamage", m_photonView.Owner, damage);
			OnLocalHit?.Invoke();
			return true;
		}

		public void ApplyHealth(float amount)
		{
			m_currentHealth = Mathf.Clamp(m_currentHealth += amount, 0, MaxHealth);
			OnChangeHealthEvent?.Invoke(m_currentHealth, MaxHealth);
		}

		private void Update()
		{
			if (m_photonView.IsMine && !m_photonView.Owner.IsKing())
			{
				Regenerate();
			}
		}

		/// <summary> Regenerate Health after certain Time.</summary>
		private void Regenerate()
		{
			if (Time.time >= m_regStartTime && m_currentHealth < MaxHealth)
			{
				m_currentHealth = Mathf.Clamp(m_currentHealth += HealthPerSeconds * Time.deltaTime, 0, MaxHealth);
				OnChangeHealthEvent?.Invoke(m_currentHealth, MaxHealth);
				m_isRegenerating = true;
				DeleteHits();
				return;
			}

			m_isRegenerating = false;
		}

		[PunRPC]
		private void ReceiveDamage(float value, PhotonMessageInfo info)
		{
			//bound to min - max
			m_currentHealth = Mathf.Clamp(m_currentHealth -= value, 0, MaxHealth);

			//Time till regeneration starts
			m_regStartTime = Time.time + TimeTillRegenerate;


			if (value > 0)
			{
				OnReceivedDamage?.Invoke(value);
			}

			if (m_photonView.IsMine)
			{
				if (m_currentHealth <= 0)
				{
					m_lastHit = info.Sender;
					m_photonView.Owner.SetAlive(false);
					OnDeath();
				}
			}

			AddHits(value, info.Sender);

			OnChangeHealthEvent?.Invoke(m_currentHealth, MaxHealth);
		}

		/// <summary>History of received Damage.</summary>
		/// <param name="value">dealed damage</param>
		/// <param name="from">player who hitted</param>
		private void AddHits(float value, Photon.Realtime.Player from)
		{
			if (!m_receivedHits.ContainsKey(from))
			{
				m_receivedHits.Add(from, value);
			}
			else
			{
				m_receivedHits[from] += value;
			}
		}

		private void DeleteHits()
		{
			if (m_currentHealth >= MaxHealth)
			{
				m_receivedHits = new Dictionary<Photon.Realtime.Player, float>();
			}
		}

		/// <summary>Change player props, create Ragdoll and destroys the Player</summary>
		private void OnDeath()
		{
			EvaluateHits(m_lastHit);

			//Player Props
			m_lastHit?.AddKill(1);
			m_lastHit?.AddScore(KillPoints);

			m_photonView.Owner.AddDeath(1);

			if (m_photonView.Owner.GetScore() >= 1)
			{
				m_photonView.Owner.AddScore(-DeathPointLose);
			}


			OnPlayerDeath?.Invoke(m_lastHit);

			PhotonNetwork.Destroy(gameObject);
		}

		/// <summary>If any Player did more damage then a specific value he gets an assist and points.</summary>
		private void EvaluateHits(Photon.Realtime.Player lastHit)
		{
			if (m_receivedHits.Count < 0) return;

			foreach (var entry in m_receivedHits)
			{
				if (m_lastHit.UserId != entry.Key.UserId && entry.Value >= DamageForAssist)
				{
					entry.Key.AddAssist(1);
					entry.Key.AddScore(AssistPoints);
				}
			}

			m_receivedHits = new Dictionary<Photon.Realtime.Player, float>();
		}

		/// <summary> Synchronize Health for all Clients </summary>
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(m_currentHealth);
				stream.SendNext(m_isRegenerating);
			}
			else
			{
				m_currentHealth = (float) stream.ReceiveNext();
				OnChangeHealthEvent?.Invoke(m_currentHealth, MaxHealth);
				m_isRegenerating = (bool) stream.ReceiveNext();
			}
		}
	}
}