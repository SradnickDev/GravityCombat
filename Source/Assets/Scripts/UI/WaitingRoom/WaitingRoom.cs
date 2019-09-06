using System.Collections;
using System.Linq;
using ExitGames.Client.Photon;
using Network.Extensions;
using Network.Gamemode;
using Network.Match;
using Photon.Pun;
using Photon.Realtime;
using SceneContainer;
using SCT;
using UI.Room;
using UI.Utilities;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace UI.WaitingRoom
{
	/// <summary>
	/// Handles Player in WaitingRoom and Timers.
	/// </summary>
	[RequireComponent(typeof(MatchSpawn))]
	[RequireComponent(typeof(MatchRespawn))]
	public class WaitingRoom : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		#region Events

		private const byte ForcedMatchStart = 76;

		#endregion

		[SerializeField] private float WaitingTime = 120.0f;
		[SerializeField] private float PreMapLoadTime = 5.0f;
		[SerializeField] private MatchSpawn MatchSpawn = null;
		[SerializeField] private MatchRespawn MatchRespawn = null;
		[SerializeField] private SceneContainerDatabase SceneContainerDatabase = null;
		[SerializeField] private SceneContainer.SceneContainer Lobby = null;
		[SerializeField] private ValueDisplaySet InfoLabel = null;
		[SerializeField] ScriptableTextDisplay ScriptableTextDisplay = null;

		private Timer m_timer = new Timer();
		private Coroutine m_pingroutine = null;
		private RoomStats m_roomStats = null;
		private bool m_matchStarting = false;
		private GameModeBase m_currentModeBase = null;

		private bool m_matchStarted = false;

		public RoomStats RoomStats
		{
			get { return m_roomStats; }
		}

		private void Start()
		{
			Cursor.lockState = CursorLockMode.None;

			m_timer.Initialize("wmt");
			m_timer.AddListener(OnTimerFinished);

			InitStats();

			m_currentModeBase = PhotonNetwork.CurrentRoom.GetGameMode();

			MatchSpawn.SetGameMode(m_currentModeBase);
			MatchRespawn.SetGameMode(m_currentModeBase);


			m_timer.Start(WaitingTime);


			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.LocalPlayer.SetTeam(Team.Aggressive);
				PhotonNetwork.CurrentRoom.IsOpen = true;
				PhotonNetwork.CurrentRoom.IsVisible = true;
				m_pingroutine = StartCoroutine(SetPing());
				InfoLabel.SetText("Press F5 to Start");
			}
			else
			{
				InfoLabel.SetText("Wait for Host to Start");
			}

			PhotonNetwork.LocalPlayer.ResetProperties();
			PhotonNetwork.LocalPlayer.SetKing(false);
		}

		public float GetTime()
		{
			if (m_timer == null)
			{
				return 0;
			}

			return m_timer.GetTime();
		}

		private void InitStats()
		{
			var room = PhotonNetwork.CurrentRoom;
			var maxPlayerCount = room.MaxPlayers;
			var currentPlayerCount = room.PlayerCount;
			var mapName = room.GetMap();
			var map = SceneContainerDatabase.GetContainerWithMapName(mapName);

			m_roomStats = new RoomStats(maxPlayerCount, currentPlayerCount, map);
		}

		#region Photon Callbacks

		/// <summary>
		/// Called when a new Player entered a Room.
		/// Masterclient assign new Player to a Team.
		/// </summary>
		/// <param name="newPlayer"></param>
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				newPlayer.SetTeam(Team.Aggressive);
			}
		}

		/// <summary>
		/// Called when a Player left a room.
		/// Update stats on all clients.
		/// </summary>
		/// <param name="newPlayer"></param>
		public override void OnPlayerLeftRoom(Player player)
		{
			UpdateRoomStats();
		}

		/// <summary>
		/// MasterClient sends spawn infos to new joined Player.
		/// If all players are ready start the Match.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="changedProps"></param>
		public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
		{
			base.OnPlayerPropertiesUpdate(target, changedProps);
			UpdateRoomStats();

			if (PhotonNetwork.IsMasterClient)
			{
				SpawnPlayer(target, changedProps);
			}

			var playerNotReady = PhotonNetwork.PlayerList.ToList().Exists(x => x.IsReady() == false);
			var room = PhotonNetwork.CurrentRoom;

			if (playerNotReady && room.MaxPlayers == room.PlayerCount && m_matchStarting == false)
			{
				OnFullRoom();
			}
		}

		#endregion

		/// <summary>
		/// If player is ready, send spawn infos.
		/// </summary>
		/// <param name="target">Target Player</param>
		/// <param name="changedProps">Changed Player properties</param>
		private void SpawnPlayer(Player target, Hashtable changedProps)
		{
			if (changedProps.TryGetValue(PlayerProperties.Ready, out var result))
			{
				var isReady = (bool) result;

				if (isReady)
				{
					MatchSpawn.SingleSpawn(target);
				}
			}
		}

		/// <summary>If MasterClient left, leave the room.</summary>
		/// <param name="newMasterClient"></param>
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			PhotonNetwork.LeaveRoom();
			LoadingScreen.LoadScene(Lobby.SceneIndex);
		}

		/// <summary>Available for MasterClient to start the match.</summary>
		public void ForceMatchStart()
		{
			if (!PhotonNetwork.IsMasterClient || m_matchStarted) return;

			m_matchStarted = true;
			foreach (var player in PhotonNetwork.PlayerList)
			{
				PhotonNetwork.RaiseEvent(ForcedMatchStart, null,
										new RaiseEventOptions
										{
											TargetActors = new[] {player.ActorNumber},
											Receivers = ReceiverGroup.All
										},
										SendOptions.SendReliable);
			}
		}

		/// <summary>Called if Room is full.</summary>
		private void OnFullRoom()
		{
			OnTimerFinished();
		}

		/// <summary>Keeps the Timer running</summary>
		private void Update()
		{
			m_timer?.Update(Time.deltaTime);

			if (Input.GetKeyDown(KeyCode.F5))
			{
				ForceMatchStart();
			}
		}

		/// <summary>Refreshes current player count</summary>
		private void UpdateRoomStats()
		{
			m_roomStats.CurrentPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		}

		/// <summary>
		/// Will be called if Timer finished or Room is Full.
		/// </summary>
		private void OnTimerFinished()
		{
			if (InfoLabel)
			{
				InfoLabel.gameObject.SetActive(false);
			}


			if (ScriptableTextDisplay != null)
			{
				ScriptableTextDisplay.InitializeScriptableText(6, transform.position, "Get Ready");
			}

			m_matchStarting = true;

			m_timer.RemoveListener(OnTimerFinished);

			if (PhotonNetwork.IsMasterClient)
			{
				LockRoom();
			}

			m_timer.Start(PreMapLoadTime);
			m_timer.AddListener(LoadPickedMap);
		}

		/// <summary>Destroy Client owned GameObjects, load cached map in room stats.</summary>
		private void LoadPickedMap()
		{
			m_timer.Deinitialize();
			m_timer.RemoveListener(LoadPickedMap);


			PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
			LoadingScreen.LoadScene(m_roomStats.Map.SceneIndex);
		}

		/// <summary>Close Room, no client will see the Room in the Lobby or can join.</summary>
		private void LockRoom()
		{
			if (m_pingroutine != null)
			{
				StopCoroutine(m_pingroutine);
			}

			PhotonNetwork.CurrentRoom.IsVisible = false;
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}

		/// <summary>Refreshes Ping for Lobby</summary>
		private IEnumerator SetPing()
		{
			PhotonNetwork.CurrentRoom.SetPing(PhotonNetwork.GetPing());
			yield return new WaitForSeconds(2);
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsOpen)
			{
				m_pingroutine = StartCoroutine(SetPing());
			}
		}

		public void OnEvent(EventData photonEvent)
		{
			switch (photonEvent.Code)
			{
				case ForcedMatchStart:
					OnTimerFinished();
					break;
			}
		}
	}
}