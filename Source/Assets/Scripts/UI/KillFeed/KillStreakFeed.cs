using System.Collections;
using SCT;
using UnityEngine;

namespace UI.KillFeed
{
	/// <summary>
	/// Used to Count and Display Kill Streaks.
	/// </summary>
	[RequireComponent(typeof(KillFeed))]
	public class KillStreakFeed : MonoBehaviour
	{
		[System.Serializable]
		public class Streak
		{
			public string Name = "";
			public int RequiredKills = 0;
		}

		[SerializeField] private Streak[] Streaks = new Streak[8];
		[SerializeField] private int MaxKillDelay = 5;
		[SerializeField] private ScriptableTextDisplay ScriptableTextDisplay = null;

		private KillFeed m_killFeed = null;
		private int m_currentKillStreak = 0;
		private Coroutine m_reset;

		private void Start()
		{
			m_killFeed = GetComponent<KillFeed>();
			m_killFeed.OnKill += OnKill;
		}

		/// <summary>
		/// Notified On Kill.
		/// Increase count, Start Timer.
		/// </summary>
		private void OnKill()
		{
			m_currentKillStreak++;

			CheackForStreak();

			ResetTimer();
		}

		/// <summary>
		/// Compare Streaks with current kill streak.
		/// </summary>
		private void CheackForStreak()
		{
			var highestStreak = new Streak();

			foreach (var streak in Streaks)
			{
				var nextStreak = streak;
				if (nextStreak.RequiredKills > highestStreak.RequiredKills)
				{
					highestStreak = nextStreak;
				}

				if (m_currentKillStreak >= nextStreak.RequiredKills)
				{
					ScriptableTextDisplay.DisableAll(8);
					ScriptableTextDisplay.InitializeScriptableText(8, transform.position, streak.Name);
				}
			}
		}

		/// <summary>
		/// Stops Coroutine to start again.
		/// </summary>
		private void ResetTimer()
		{
			if (m_reset != null)
			{
				StopCoroutine(m_reset);
			}

			m_reset = StartCoroutine(Reset());
		}

		/// <summary>
		/// Wait for a short amount of time to reset current kill count
		/// </summary>
		/// <returns></returns>
		private IEnumerator Reset()
		{
			yield return new WaitForSeconds(MaxKillDelay);
			m_currentKillStreak = 0;
		}

		private void OnDisable()
		{
			if (m_killFeed != null)
			{
				m_killFeed.OnKill -= OnKill;
			}
		}
	}
}