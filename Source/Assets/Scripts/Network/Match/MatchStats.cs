using System;
using System.Collections.Generic;
using Network.Extensions;
using Network.Gamemode;
using Photon.Pun;
using UI;
using UI.Match;
using UI.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Network.Match
{
	/// <summary>
	/// Displays Timer and stats for specific GameModes.
	/// </summary>
	[RequireComponent(typeof(MatchModel))]
	[RequireComponent(typeof(StartNextRound))]
	public class MatchStats : MonoBehaviour
	{
		[SerializeField] private MatchModel MatchModel = null;
		[SerializeField] private StartNextRound StartNextRound = null;
		[SerializeField] private ValueDisplaySet Timer = null;
		[SerializeField] private ValueDisplaySet LeftSide = null;
		[SerializeField] private ValueDisplaySet RightSide = null;
		[SerializeField] private ValueDisplaySet Counter = null;
		[SerializeField] private GameObject BarContainer = null;
		[SerializeField] private Image Bar = null;
		[SerializeField] private TimerWarning TimerWarning = null;

		private Timer m_timer = null;
		private GameModeBase m_currentModeBase = null;

		private void OnEnable()
		{
			Initialize();
			if (MatchModel != null)
			{
				MatchModel.OnTimerStarted += SetTimer;
			}

			StartNextRound.OnTimerStarted += SetTimer;
			SetGameMode();
		}

		private void Initialize()
		{
			LeftSide.gameObject.SetActive(false);
			RightSide.gameObject.SetActive(false);
			Counter.gameObject.SetActive(false);
			BarContainer.SetActive(false);
		}

		/// <summary>
		/// Triggered from an Event in Match Model
		/// </summary>
		/// <param name="timer"></param>
		private void SetTimer(Timer timer)
		{
			m_timer = timer;
		}

		/// <summary>
		/// Load GameMode from room properties.
		/// </summary>
		private void SetGameMode()
		{
			m_currentModeBase = PhotonNetwork.CurrentRoom.GetGameMode();
		}

		private void Update()
		{
			if (m_currentModeBase == null) return;

			SetTimer();

			HandleStats();
		}

		/// <summary>Displays given Time.</summary>
		private void SetTimer()
		{
			if (m_timer != null)
			{
				var formattedTime = FormatTime();
				Timer.SetText(formattedTime);
			}
		}

		/// <summary>Converts seconds to minutes and seconds.</summary>
		private string FormatTime()
		{
			var time = TimeSpan.FromSeconds(m_timer.GetTime());
			var formattedTime = $"{time.Minutes:D2}:{time.Seconds:D2}";

			HandleWarning(time);

			return formattedTime;
		}

		/// <summary>
		/// Takes care for the time. Starts warning mode and turns off if needed.
		/// </summary>
		/// <param name="time"></param>
		private void HandleWarning(TimeSpan time)
		{
			if (time.Minutes == 0 && time.Seconds <= 5)
			{
				TimerWarning.Start();
			}
			else
			{
				TimerWarning.Stop();
			}
		}

		/// <summary>Summarize stats Handling.</summary>
		private void HandleStats()
		{
			string[] stats = m_currentModeBase.GetStats();
			FriendlyStats(stats);
			OpponentStats(stats);
			CounterStats(stats);
			BarStats(stats);
		}

		/// <summary>Takes care of Friendly/Local stats if available.</summary>
		/// <param name="stats">Stats from current GameMode</param>
		private void FriendlyStats(IReadOnlyList<string> stats)
		{
			if (stats[0] == null) return;
			LeftSide.gameObject.SetActive(true);
			LeftSide.SetText(stats[0]);
		}

		/// <summary>Takes care of Opponent stats if available.</summary>
		/// <param name="stats">Stats from current GameMode</param>
		private void OpponentStats(IReadOnlyList<string> stats)
		{
			if (stats[1] == null) return;
			RightSide.gameObject.SetActive(true);
			RightSide.SetText(stats[1]);
		}

		/// <summary>Takes care of Counter stats if available.</summary>
		/// <param name="stats">Stats from current GameMode</param>
		private void CounterStats(IReadOnlyList<string> stats)
		{
			if (stats[2] == null) return;
			Counter.gameObject.SetActive(true);
			Counter.SetText(stats[2]);
		}

		/// <summary>Takes care of Bar stats if available.</summary>
		/// <param name="stats">Stats from current GameMode</param>
		private void BarStats(IReadOnlyList<string> stats)
		{
			if (stats[3] == null) return;
			BarContainer.SetActive(true);

			if (float.TryParse(stats[3], out var value))
			{
				Bar.fillAmount = value / KillTheKing.KingMaxHealth;
			}
		}
	}
}