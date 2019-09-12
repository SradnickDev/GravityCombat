using System;
using Network.Extensions;
using Network.Gamemode;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Network.Match
{
	/// <summary>
	/// Small wrapper to simplify player spawning.
	/// </summary>
	public class MatchSpawn : MonoBehaviour
	{
		private GameModeBase m_currentModeBase = null;

		private void Awake()
		{
			m_currentModeBase = PhotonNetwork.CurrentRoom.GetGameMode();
		}

		/// <summary>
		///  Send Spawn Event to all Clients.
		/// </summary>
		public void SpawnAll()
		{
			if (!PhotonNetwork.IsMasterClient) return;

			Player[] photonPlayerList = PhotonNetwork.PlayerList;

			foreach (var pp in photonPlayerList)
			{
				SingleSpawn(pp);
			}
		}

		/// <summary>
		/// Spawn a single Player based on GameMode and Team.
		/// </summary>
		/// <param name="player"></param>
		public void SingleSpawn(Player player)
		{
			if (m_currentModeBase != null)
			{
				m_currentModeBase.SpawnPlayer(player);
			}
		}
	}
}