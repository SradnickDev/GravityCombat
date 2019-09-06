using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WaitingRoom
{
	[RequireComponent(typeof(WaitingRoom))]
	public class WaitingRoomStats : MonoBehaviour
	{
		[SerializeField] private Text TimerDisplay = null;
		[SerializeField] private Text PlayerCountDisplay = null;

		private WaitingRoom m_waitingRoom = null;

		private void Start()
		{
			m_waitingRoom = GetComponent<WaitingRoom>();
		}

		private void Update()
		{
			UpdateStats();
		}

		private void UpdateStats()
		{
			if (m_waitingRoom == null || m_waitingRoom.RoomStats == null) return;

			var stats = m_waitingRoom.RoomStats;
			SetPlayerCount(stats.CurrentPlayerCount, stats.MaxPlayerCount);
			var time = m_waitingRoom.GetTime();
			SetTime(m_waitingRoom.GetTime());
		}

		private void SetPlayerCount(int current, int max)
		{
			if (PlayerCountDisplay == null) return;

			PlayerCountDisplay.text = $"{current}/{max}";
		}

		private void SetTime(float sec)
		{
			if (TimerDisplay == null) return;

			var time = TimeSpan.FromSeconds(sec);
			var formattedTime = $"{time.Minutes:D2}:{time.Seconds:D2}";
			TimerDisplay.text = formattedTime;
		}
	}
}