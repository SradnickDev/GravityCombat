using Photon.Pun;
using UnityEngine;
using System.Linq;
using Network.Match;
using Photon.Realtime;
using Network.Extensions;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Network.WaitingRoom
{
	/// <summary>
	/// Wraps up photon callbacks and sync specific data between all clients to easier create a Waiting Room like Behaviour.
	/// Provides only really needed Callbacks, than can be overwritten.
	/// </summary>
	public class WaitingRoomPart : MonoBehaviour, IInRoomCallbacks, IOnEventCallback, IWaitingRoomCallback
	{
		
		private static class WaitingRoomEvents
		{
			public const byte StateChanged = 110;
			public const byte StateRequest = 111;
		}

		[SerializeField] private float WaitingTime = 120.0f;
		[SerializeField] private float PreMapLoadTime = 5.0f;
		/// <summary>
		/// Current State of the Waiting Room.
		/// </summary>
		private WaitingRoomState m_waitingRoomState = WaitingRoomState.None;
		private Timer m_timer = null;
		private bool m_started = false;

		#region Properties

		public WaitingRoomState State => m_waitingRoomState;

		/// <summary>
		/// Max Players in this Room, set via Room properties
		/// </summary>
		public int MaxPlayerCount => PhotonNetwork.CurrentRoom.MaxPlayers;

		/// <summary>
		/// Current available Players
		/// </summary>
		public int CurrentPlayerCount => PhotonNetwork.CurrentRoom.PlayerCount;

		public float TimeLeft => m_timer.GetTime();

		#endregion

		/// <summary>
		/// Used to initialize the Timer and set the first state.
		/// None Masterclient only request the current state.
		/// </summary>
		/// <param name="waitingTime">Max Time to wait</param>
		/// <param name="preloadTime">Time before match should start</param>
		public virtual void Start()
		{
			m_timer = new Timer();
			PhotonNetwork.AddCallbackTarget(this);
			m_timer.Initialize("wrt");

			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.CurrentRoom.IsOpen = true;
				PhotonNetwork.CurrentRoom.IsVisible = true;


				m_timer.AddListener(OnWaitingTimerEnds);
				m_timer.Start(WaitingTime);
				ChangeState(WaitingRoomState.WaitingForPlayer);
				PlayerCountChanged();
			}
			else
			{
				RequestState();
			}
		}

		public virtual void Update()
		{
			m_timer.Update(Time.deltaTime);
		}

		/// <summary>
		/// Called when first Timer ends, the room will be closed and not visible anymore.
		/// Stats also the preload Timer.
		/// </summary>
		private void OnWaitingTimerEnds()
		{
			if (!PhotonNetwork.IsMasterClient) return;

			CloseRoom();
			ChangeState(WaitingRoomState.TimerIsOver);

			m_timer.Stop();
			m_timer.RemoveListener(OnWaitingTimerEnds);
			m_timer.AddListener(OnPreloadTimerEnds);

			m_timer.Start(PreMapLoadTime);
		}

		/// <summary>
		/// If preload Timer ends, Match can start.
		/// </summary>
		private void OnPreloadTimerEnds()
		{
			BeforeStartMatch();
		}

		/// <summary>
		/// Raises an Event with the new current State that all client will receive.
		/// </summary>
		/// <param name="targetState">current Waiting Room state</param>
		private void ChangeState(WaitingRoomState targetState)
		{
			if (PhotonNetwork.IsMasterClient && m_waitingRoomState != targetState)
			{
				foreach (var player in PhotonNetwork.CurrentRoom.Players)
				{
					PhotonNetwork.RaiseEvent(WaitingRoomEvents.StateChanged, new object[] {targetState},
											new RaiseEventOptions
											{
												TargetActors = new[] {player.Value.ActorNumber},
												Receivers = ReceiverGroup.All,
												CachingOption = EventCaching.DoNotCache
											},
											SendOptions.SendReliable);
				}
			}
		}

		/// <summary>
		/// Can be used to force the Waiting Room to start a Match.
		/// </summary>
		public void ForcedStart()
		{
			OnWaitingTimerEnds();
		}

		/// <summary>
		/// if Room is Full, match will be started.
		/// </summary>
		private void PlayerCountChanged()
		{
			if (MaxPlayerCount == CurrentPlayerCount && m_waitingRoomState !=
				WaitingRoomState.Full && m_waitingRoomState != WaitingRoomState.TimerIsOver)
			{
				OnWaitingTimerEnds();
			}
		}

		/// <summary>
		/// Raises an event that the Match can start.
		/// </summary>
		private void BeforeStartMatch()
		{
			m_started = true;
			VerifyStartMatch();
		}

		private void CloseRoom()
		{
			PhotonNetwork.CurrentRoom.IsVisible = false;
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}

		/// <summary>
		/// Client request current Waiting Room state from Masterclient.
		/// </summary>
		private void RequestState()
		{
			PhotonNetwork.RaiseEvent(WaitingRoomEvents.StateRequest, null,
									new RaiseEventOptions
									{
										TargetActors = new[] {PhotonNetwork.MasterClient.ActorNumber},
										Receivers = ReceiverGroup.All
									},
									SendOptions.SendReliable);
		}

		protected virtual void OnStateChanged(WaitingRoomState state)
		{
			var receivedState = state;
			m_waitingRoomState = receivedState;

			if (m_waitingRoomState == WaitingRoomState.Full)
			{
				OnWaitingTimerEnds();
			}

			if (m_waitingRoomState == WaitingRoomState.StartMatch)
			{
				MatchStart();
			}
		}

		//Use these as callback to implement specific behaviour in derived classes
		//to simplify the use of photon even more

		#region Callbacks

		/// <summary>
		/// Called when a new player joined
		/// </summary>
		public virtual void PlayerJoined(Player player) { }

		/// <summary>
		/// Called
		/// </summary>
		public virtual void PlayerLeft(Player player) { }

		public virtual void MasterClientLeft() { }

		public virtual void PlayerIsReady(Player player) { }

		public virtual void MatchStart() { }

		#endregion

		#region Photon Callbacks

		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			if (!PhotonNetwork.IsMasterClient) return;

			if (changedProps.TryGetValue(PlayerProperties.Ready, out var isReady))
			{
				if ((bool) isReady)
				{
					PlayerIsReady(targetPlayer);
				}
			}

			VerifyStartMatch();
		}

		private void VerifyStartMatch()
		{
			if (!m_started) return;

			if (PhotonNetwork.CurrentRoom.Players.Any(player => !player.Value.IsReady()))
			{
				return;
			}

			ChangeState(WaitingRoomState.StartMatch);
		}

		public void OnEvent(EventData photonEvent)
		{
			switch (photonEvent.Code)
			{
				case WaitingRoomEvents.StateRequest:

					PhotonNetwork.RaiseEvent(WaitingRoomEvents.StateRequest, new object[] {m_waitingRoomState},
											new RaiseEventOptions
											{
												TargetActors = new[] {photonEvent.Sender},
												Receivers = ReceiverGroup.All
											},
											SendOptions.SendReliable);

					break;
				case WaitingRoomEvents.StateChanged:
					var content = (object[]) photonEvent.CustomData;
					OnStateChanged((WaitingRoomState) content[0]);

					break;
			}
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			PlayerCountChanged();
			PlayerJoined(newPlayer);
		}

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			PlayerCountChanged();
			PlayerLeft(otherPlayer);
		}

		public void OnMasterClientSwitched(Player newMasterClient)
		{
			MasterClientLeft();
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }

		#endregion

		public virtual void OnDisable()
		{
			m_timer.Deinitialize();
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}
}