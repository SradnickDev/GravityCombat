using System.Collections;
using Network.Gamemode;
using Photon.Pun;
using Photon.Realtime;
using PlayerBehaviour.Model;
using SCT;
using UnityEngine;

namespace Network.Match
{
	/// <summary>
	/// Respawn mechanic + timer
	/// </summary>
	[RequireComponent(typeof(MatchSpawn))]
	public class MatchRespawn : MonoBehaviour
	{
		[SerializeField] private int RespawnTime = 5;
		[SerializeField] private bool IgnoreGamemode = false;
		[SerializeField] private ScriptableTextDisplay ScriptableTextDisplay = null;

		private PlayerHealthModel m_healthHandler = null;
		private GameModeBase m_currentModeBase = null;
		private MatchSpawn m_matchSpawn = null;
		private Coroutine m_coroutine = null;

		#region Setup

		private void Start()
		{
			m_matchSpawn = GetComponent<MatchSpawn>();
		}

		public void Init(PlayerHealthModel healthModel)
		{
			m_healthHandler = healthModel;
			healthModel.OnPlayerDeath += StartCountdown;
		}

		private void OnDisable()
		{
			if (m_healthHandler != null)
			{
				m_healthHandler.OnPlayerDeath -= StartCountdown;
			}
		}

		public void SetGameMode(GameModeBase modeBase)
		{
			m_currentModeBase = modeBase;
		}

		#endregion

		/// <summary>
		/// Initialize respawn Countdown.
		/// </summary>
		/// <param name="lastHit"></param>
		private void StartCountdown(Player lastHit)
		{
			if (!m_currentModeBase.AllowRespawn() && !IgnoreGamemode) return;

			m_coroutine = StartCoroutine(Countdown());
		}

		/// <summary>
		/// Stops immediately respawn coroutine (respawning). 
		/// </summary>
		public void StopSpawn()
		{
			if (m_coroutine != null)
			{
				StopCoroutine(m_coroutine);
			}
		}

		private IEnumerator Countdown()
		{
			var duration = RespawnTime;
			while (duration != -1)
			{
				ScriptableTextDisplay.InitializeScriptableText(4, Vector3.zero,
																"Respawn in " + duration.ToString("F0"));
				yield return new WaitForSeconds(1);
				duration--;
				if (duration == 0)
				{
					SpawnEvents.RespawnRandomSpawnNode(PhotonNetwork.LocalPlayer);
				}
			}
		}
	}
}