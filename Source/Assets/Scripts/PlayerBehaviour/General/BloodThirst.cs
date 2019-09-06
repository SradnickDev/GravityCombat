using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;
using PlayerBehaviour.Model;
using SCT;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace PlayerBehaviour.General
{
	[RequireComponent(typeof(PlayerHealthModel))]
	public class BloodThirst : MonoBehaviourPunCallbacks
	{
		[SerializeField] private PlayerHealthModel m_healthModel = null;
		[SerializeField] private float HealthOnKill = 30;
		[SerializeField] private float KingAdditionalHeal = 20;
		[SerializeField] private PhotonView PhotonView = null;
		private int m_previousKills = 0;

		private void Start()
		{
			m_healthModel = GetComponent<PlayerHealthModel>();
		}

		/// <summary>
		/// Photon Callback for changed player properties.
		/// To check if from local player properties changed.
		/// </summary>
		/// <param name="target">Target Player</param>
		/// <param name="changedProps">changed properties</param>
		public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
		{
			var samePlayer = target.ActorNumber == PhotonView.Owner.ActorNumber;
			var localOwner = PhotonView.IsMine;

			if (!samePlayer || !localOwner) return;

			if (changedProps.TryGetValue(PlayerProperties.Kills, out var changedValue))
			{
				if ((int) changedValue > m_previousKills)
				{
					OnPropertyChanged(changedValue);
				}
			}
		}

		/// <summary>
		/// Self heal if correct property changed.
		/// </summary>
		/// <param name="value"></param>
		private void OnPropertyChanged(object value)
		{
			var heal = HealthOnKill;

			if (PhotonView.Owner.IsKing())
			{
				heal += KingAdditionalHeal;
			}

			m_healthModel.ApplyHealth(heal);
			ScriptableTextDisplay.Instance.InitializeScriptableText(9, transform.position, $"+{heal}");
			m_previousKills = (int) value;
		}
	}
}