using System;
using ExitGames.Client.Photon;
using Network.Extensions;
using Network.Match;
using Photon.Pun;
using SceneContainer;
using SCT;
using UnityEngine;

namespace UI.Match
{
	public class StartNextRound : MonoBehaviourPunCallbacks
	{
		#region Events

		public Action<Timer> OnTimerStarted;

		#endregion

		[SerializeField] private SceneContainerDatabase SceneContainerDatabase = null;
		[SerializeField] private SceneContainer.SceneContainer LobbyContainer = null;
		[SerializeField] private int NextRoundTime = 10;
		[SerializeField] private int TimerAfterLastRound = 25;
		[SerializeField] private ScriptableTextDisplay ScriptableTextDisplay = null;

		private Timer m_timer = new Timer();

		/// <summary>
		/// Unity callback.
		/// Connect Timer stuff
		/// </summary>
		public override void OnEnable()
		{
			base.OnEnable();
			var key = "nrt" + PhotonNetwork.CurrentRoom.GetRounds();
			m_timer.Initialize(key);
			m_timer.AddListener(OnTimerEnds);
		}

		/// <summary>
		/// Unity callback.
		/// Disconnect Timer stuff
		/// </summary>
		public override void OnDisable()
		{
			base.OnDisable();
			m_timer.RemoveListener(OnTimerEnds);
			m_timer.RemoveListener(OnLastRoundEnds);
			m_timer.Deinitialize();
		}

		/// <summary>
		/// Update Timer.
		/// </summary>
		private void Update()
		{
			m_timer.Update(Time.deltaTime);
		}

		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			if (PhotonNetwork.IsMasterClient) return;

			foreach (var key in propertiesThatChanged.Keys)
			{
				if ((string) key == RoomProperties.Round)
				{
					OnRoundEnds();
				}
			}
		}

		/// <summary>
		/// Starts a timer after a match finished and enough rounds left.
		/// </summary>
		public void StartTimer()
		{
			if (!PhotonNetwork.IsMasterClient) return;
			DecreaseRounds();

			OnRoundEnds();
		}

		private void OnRoundEnds()
		{
			if (!CanStartNewRound())
			{
				//Reset to default
				m_timer.Reset();

				//reusing for leaving room instead of new round
				m_timer.AddListener(OnLastRoundEnds);
				m_timer.Start(TimerAfterLastRound);

				OnTimerStarted?.Invoke(m_timer);
				ScriptableTextDisplay.InitializeScriptableText(6, transform.position, "Combat is over!");
				return;
			}

			if (ScriptableTextDisplay != null)
			{
				ScriptableTextDisplay.InitializeScriptableText(6, transform.position, "Next Round");
			}

			m_timer.Start(NextRoundTime);
			OnTimerStarted?.Invoke(m_timer);
		}

		/// <summary>
		/// Go back to Lobby.
		/// </summary>
		private void OnLastRoundEnds()
		{
			PhotonNetwork.LeaveRoom();
			LoadingScreen.LoadScene(LobbyContainer.SceneIndex);
		}

		/// <summary>
		/// Reload the current Map, if Timer ends.
		/// </summary>
		private void OnTimerEnds()
		{
			PhotonNetwork.LocalPlayer.SetTeam(Team.None);
			var map = SceneContainerDatabase.GetContainerWithMapName(PhotonNetwork.CurrentRoom.GetMap());
			LoadingScreen.LoadScene(map.SceneIndex);
		}

		/// <summary>
		/// Check if Room Properties if enough rounds left
		/// </summary>
		/// <returns>True if more than 0, else False</returns>
		private bool CanStartNewRound()
		{
			return PhotonNetwork.CurrentRoom.GetRounds() > 0;
		}

		/// <summary>
		/// Decrease Rounds from Room Properties with 1.
		/// </summary>
		private void DecreaseRounds()
		{
			var currentRounds = PhotonNetwork.CurrentRoom.GetRounds();
			PhotonNetwork.CurrentRoom.SetRounds(currentRounds - 1);
		}
	}
}