using System;
using System.Linq;
using Network.Extensions;
using Network.Gamemode;
using Photon.Pun;
using Photon.Realtime;
using SCT;
using UI;
using UI.Match;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Network.Match
{
	/// <summary>
	/// Logic for Match.
	/// </summary>
	[RequireComponent(typeof(MatchSpawn))]
	[RequireComponent(typeof(MatchRespawn))]
	[RequireComponent(typeof(ReadyOnSceneLoaded))]
	[RequireComponent(typeof(StartNextRound))]
	public class MatchModel : MonoBehaviourPunCallbacks
	{
		#region Events

		public Action OnStart;
		public Action<GameModeBase> OnReceivedGameMode;
		public Action<Timer> OnTimerStarted;
		public Action OnMatchStart;
		public Action OnMatchEnded;

		#endregion

		[SerializeField] private int PreloadTime = 5;
		[SerializeField] private SceneContainer.SceneContainer LobbyContainer = null;
		[SerializeField] private ScriptableTextDisplay ScriptableTextDisplay = null;

		private MatchSpawn m_matchSpawn = null;
		private MatchRespawn m_matchRespawn = null;
		private GameModeBase m_currentModeBase = null;
		private StartNextRound m_nextRound = null;

		//has to be false to start preLoading, do not change!
		private bool m_preLoading = false;

		private readonly Timer m_preloadTimer = new Timer();
		private readonly Timer m_timer = new Timer();
		private bool m_matchStarted = false;
		private bool m_spawned = false;

		private void Awake()
		{
			m_matchSpawn = GetComponent<MatchSpawn>();
			m_matchRespawn = GetComponent<MatchRespawn>();
			m_nextRound = GetComponent<StartNextRound>();

			ResetPlayerProperties();
		}

		private void Start()
		{
			OnStart?.Invoke();
			SetupGameMode();
			InitializeTimer();
		}

		private void InitializeTimer()
		{
			m_preloadTimer.Initialize("prlot");
			m_preloadTimer.AddListener(OnPreloadTimerFinished);

			m_timer.Initialize("omati");
			m_timer.AddListener(OnMatchTimerFinished);
		}

		/// <summary>
		/// Load Game Mode and initialize components.
		/// </summary>
		private void SetupGameMode()
		{
			m_currentModeBase = PhotonNetwork.CurrentRoom.GetGameMode();
			m_currentModeBase.HandlePlayer();

			if (m_currentModeBase is KillTheKing)
			{
				(m_currentModeBase as KillTheKing).SetHealth();
			}

			OnReceivedGameMode?.Invoke(m_currentModeBase);

			m_currentModeBase.OnConditionReached += OnMatchEnd;

			m_matchSpawn.SetGameMode(m_currentModeBase);
			m_matchRespawn.SetGameMode(m_currentModeBase);
		}

		/// <summary>
		/// Sets player properties to default values.
		/// </summary>
		private void ResetPlayerProperties()
		{
			foreach (var player in PhotonNetwork.PlayerList)
			{
				player.ResetProperties();
			}
		}

		/// <summary> Check if every Player is Ready.</summary>
		/// <param name="targetPlayer">Player that changed properties</param>
		/// <param name="changedProps">only changed properties</param>
		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			//false if one player is not ready
			var playerNotReady = PhotonNetwork.PlayerList.ToList().Exists(x => x.IsReady() == false);

			//Start preload timer if all clients are done with scene loading
			if (playerNotReady == false && !m_preLoading)
			{
				m_preloadTimer.Start(PreloadTime);
				m_preLoading = true;
				OnTimerStarted?.Invoke(m_preloadTimer);

				m_currentModeBase.Start();

				if (ScriptableTextDisplay != null)
				{
					ScriptableTextDisplay.InitializeScriptableText(6, transform.position, "Get Ready !");
				}
			}
		}

		/// <summary>
		/// Called when Preload Timer finished. Send Spawn Event to all Clients.
		/// Starts Match Timer.
		/// </summary>
		private void OnPreloadTimerFinished()
		{
			m_preloadTimer.Deinitialize();
			m_preloadTimer.RemoveListener(OnMatchTimerFinished);

			if (PhotonNetwork.IsMasterClient && !m_spawned)
			{
				m_matchSpawn.SpawnAll();
				m_spawned = true;
			}

			if (ScriptableTextDisplay != null)
			{
				ScriptableTextDisplay.InitializeScriptableText(6, transform.position, "GO !");
			}

			OnTimerStarted?.Invoke(m_timer);
			OnMatchStart?.Invoke();
			m_matchStarted = true;

			m_timer.Start(PhotonNetwork.CurrentRoom.GetMatchTime());
		}

		private void Update()
		{
			m_preloadTimer.Update(Time.deltaTime);
			m_timer.Update(Time.deltaTime);

			if (m_matchStarted)
			{
				m_currentModeBase.UpdateWinCondition();
			}
		}

		/// <summary>
		/// Called when Match is over from Match Timer
		/// </summary>
		private void OnMatchTimerFinished()
		{
			OnMatchEnd();
		}

		/// <summary>
		/// Called at the end of a match.
		/// Destroys Player Objects and starts end timer
		/// </summary>
		private void OnMatchEnd()
		{
			PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
			m_matchStarted = false;
			m_timer.Stop();

			m_matchRespawn.StopSpawn();

			OnMatchEnded?.Invoke();
			m_nextRound.StartTimer();
		}

		/// <summary>
		/// If MasterClient left the room , leave room.
		/// </summary>
		/// <param name="newMasterClient"></param>
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			PhotonNetwork.LeaveRoom();
			LoadingScreen.LoadScene(LobbyContainer.SceneIndex);
		}

		public override void OnDisable()
		{
			base.OnDisable();
			m_timer.RemoveListener(OnMatchTimerFinished);
			m_timer.Deinitialize();
			m_currentModeBase.OnConditionReached -= OnMatchEnd;
		}
	}
}