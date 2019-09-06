using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Network.Match
{
	/// <summary>
	/// PhotonNetwork Timer, using Photon Events(RaiseEvents).
	/// Call Initialize() as soon as the Timer should react to Photon Callbacks(RaiseEvents), Deinitialize if not needed.
	/// Every Client should Update the Timer e.g. timer.Update(Time.deltaTime)
	/// Only MasterClient can Start/Stop , send Finished Event.
	/// Each client is able to reset the Timer.
	/// </summary>
	public class Timer : IOnEventCallback
	{
		private static class MatchTimerEvent
		{
			public const byte Started = 63;
			public const byte Finished = 65;
			public const byte Stopped = 66;
		}

		#region Events

		private Action OnFinished;

		#endregion

		private bool m_started = false;
		private float m_currentTime = 0.0f;
		private string m_key = "";
		private float m_timeStamp = 0.0f;

		#region Init

		/// <summary>Register to Photons Callback </summary>
		/// <param name="key">Unique key to identify the Timer</param>
		public void Initialize(string key)
		{
			PhotonNetwork.AddCallbackTarget(this);
			m_key = key;
		}

		/// <summary> Remove Callback connection.</summary>
		public void Deinitialize()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				RemoveCachedStartEvent();
			}

			PhotonNetwork.RemoveCallbackTarget(this);
		}

		/// <summary> Register Listener, will be called when Timer Finished</summary>
		public void AddListener(Action action)
		{
			OnFinished += action;
		}

		/// <summary>Remove Listener</summary>
		public void RemoveListener(Action action)
		{
			if (OnFinished != null)
			{
				OnFinished -= action;
			}
		}

		#endregion

		#region Main

		/// <summary>
		/// Starts Timer with given Time.
		/// </summary>
		public void Start(float newTime)
		{
			if (!PhotonNetwork.IsMasterClient) return;

			m_timeStamp = (float) PhotonNetwork.ServerTimestamp;
			SendStartEvent(MatchTimerEvent.Started, new object[] {m_key, m_timeStamp, newTime});
		}

		/// <summary>
		/// Stops Timer, does not Trigger OnFinished.
		/// Clear Cache.
		/// </summary>
		public void Stop()
		{
			if (!PhotonNetwork.IsMasterClient) return;

			RemoveCachedStartEvent();
			SendEvent(MatchTimerEvent.Stopped, new object[] {m_key});
		}

		/// <summary>
		/// Reset current Time, timeStamp. Removes listeners and cached Events.
		/// </summary>
		public void Reset()
		{
			m_timeStamp = 0;
			m_started = false;
			m_currentTime = 0;
			OnFinished = null;
			RemoveCachedStartEvent();
		}

		/// <summary>Timer needs be updated in Update using Time.deltaTime.</summary>
		/// <param name="delta">Time.deltaTime</param>
		public void Update(float delta)
		{
			if (!m_started) return;

			m_currentTime -= delta;

			if (m_currentTime <= 0)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					SendEvent(MatchTimerEvent.Finished, new object[] {m_key});
				}

				m_currentTime = 0.0f;
				m_started = false;
			}
		}

		/// <summary>Current Time.</summary>
		public float GetTime()
		{
			return m_currentTime;
		}

		#endregion

		#region Events stuff

		/// <summary>
		/// Only accessible through Photon interface IOnEventCallback.
		/// </summary>
		/// <param name="photonEvent"></param>
		public void OnEvent(EventData photonEvent)
		{
			object[] eventContent;
			var key = "";

			switch (photonEvent.Code)
			{
				case MatchTimerEvent.Started:
					eventContent = (object[]) photonEvent.CustomData;

					key = (string) eventContent[0];
					if (key != m_key) return;

					var serverTimeStamp = (float) eventContent[1];
					var newTime = (float) eventContent[2];

					m_currentTime = newTime;
					SetRealMatchTime(serverTimeStamp);


					m_started = true;
					break;

				case MatchTimerEvent.Finished:
					eventContent = (object[]) photonEvent.CustomData;

					key = (string) eventContent[0];
					if (key != m_key) return;

					OnFinished?.Invoke();

					break;

				case MatchTimerEvent.Stopped:
					eventContent = (object[]) photonEvent.CustomData;

					key = (string) eventContent[0];
					if (key != m_key) return;
					m_started = false;
					m_currentTime = 0;

					break;
			}
		}

		/// <summary>Only for Start event, it has to be cached to the Room.</summary>
		/// <param name="eventCode">Start Event Key</param>
		/// <param name="param">Key and Timestamp</param>
		private void SendStartEvent(byte eventCode, object[] param)
		{
			//Added to room cache, new Clients will receive this event immediately.
			PhotonNetwork.RaiseEvent(eventCode, param,
									new RaiseEventOptions
									{
										CachingOption = EventCaching.AddToRoomCache,
										Receivers = ReceiverGroup.All
									},
									SendOptions.SendReliable);
		}

		/// <summary>Only Stopped or Finished event.</summary>
		/// <param name="eventCode">Stopped/Finished Event code</param>
		/// <param name="param">Key is needed</param>
		private void SendEvent(byte eventCode, object[] param)
		{
			if (!PhotonNetwork.InRoom) return;

			foreach (var player in PhotonNetwork.PlayerList)
			{
				PhotonNetwork.RaiseEvent(eventCode, param,
										new RaiseEventOptions
										{
											TargetActors = new[] {player.ActorNumber},
											Receivers = ReceiverGroup.All
										},
										SendOptions.SendReliable);
			}
		}

		/// <summary>Removes Started Event from Room cache.</summary>
		private void RemoveCachedStartEvent()
		{
			if (!PhotonNetwork.InRoom) return;
			PhotonNetwork.RaiseEvent(MatchTimerEvent.Started, new object[] {m_key, m_timeStamp},
									new RaiseEventOptions
									{
										CachingOption = EventCaching.RemoveFromRoomCache,
										Receivers = ReceiverGroup.All
									},
									SendOptions.SendReliable);
		}

		/// <summary>
		/// Calculate difference between event raise and event receive.
		/// </summary>
		/// <param name="timeStamp">Event Raise Time</param>
		private void SetRealMatchTime(float timeStamp)
		{
			var dif = (PhotonNetwork.ServerTimestamp - timeStamp) / 1000.0f;
			if (!Mathf.Approximately(timeStamp, 0))
			{
				m_currentTime -= dif;
			}
		}

		#endregion
	}
}