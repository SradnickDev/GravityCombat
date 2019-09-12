using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WaitingRoom
{
	[RequireComponent(typeof(WaitingRoom))]
	public class WaitingRoomStatsView : MonoBehaviour
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
			if (m_waitingRoom == null) return;

			SetPlayerCount(m_waitingRoom.CurrentPlayerCount, m_waitingRoom.MaxPlayerCount);

			var time = m_waitingRoom.TimeLeft;
			SetTime(time);
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