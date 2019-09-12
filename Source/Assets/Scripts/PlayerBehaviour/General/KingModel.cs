using Network.Extensions;
using Photon.Pun;
using PlayerBehaviour.Model;
using UnityEngine;

namespace PlayerBehaviour.General
{
	[RequireComponent(typeof(PlayerHealthModel))]
	public class KingModel : MonoBehaviour
	{
		[SerializeField] private float ScaleFactor = 1.2f;
		[SerializeField] private ParticleSystem Effect = null;

		private PlayerHealthModel m_playerHealthModel = null;
		private PhotonView m_photonView = null;
		private bool m_isDead = false;

		private void Start()
		{
			m_playerHealthModel = GetComponent<PlayerHealthModel>();
			m_photonView = GetComponent<PhotonView>();

			if (m_photonView.Owner.IsKing())
			{
				m_playerHealthModel.OnChangeHealthEvent += HealthChanged;
				SetScale();

				if (Effect != null)
				{
					Effect.Play(true);
				}
			}
		}

		private void SetScale()
		{
			var tempScale = transform.localScale;
			tempScale *= ScaleFactor;
			transform.localScale = tempScale;
		}

		private void HealthChanged(float current, float max)
		{
			if (m_isDead) return;

			if (current <= 0)
			{
				m_isDead = true;
				current = 0;
			}

			PhotonNetwork.CurrentRoom.SetKingHealth(current);
		}

		private void OnDestroy()
		{
			if (!PhotonNetwork.InRoom) return;

			if (m_photonView.Owner.IsKing())
			{
				PhotonNetwork.CurrentRoom.SetKingHealth(0);
				m_photonView.Owner.SetKing(false);
			}
		}
	}
}