using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Network.Gamemode
{
	/// <summary>
	/// One Client will be picked as the King.
	/// Get Huge amount of Health.
	/// GameMode win if King is dead.
	/// </summary>
	public sealed class KillTheKing : GameModeBase
	{
		public static float KingMaxHealth
		{
			get { return 500 * PhotonNetwork.CurrentRoom.PlayerCount; }
		}

		private Player m_king = null;


		/// <summary>
		/// All Player join one Team, King joins a other Team.
		/// </summary>
		/// <param name="player"></param>
		protected override void Assign(Player player)
		{
			Stats[3] = "0";
			PhotonNetwork.CurrentRoom.SetKingHealth(KingMaxHealth);
			
			if (m_king != null && m_king.ActorNumber == player.ActorNumber) return;

			Teams.AssignTo(player, Team.White);
		}

		///<summary>Random king will be picked.</summary>
		private void SetKing()
		{
			if (m_king != null) return;

			Player[] playerInRoom = PhotonNetwork.PlayerList;
			m_king = playerInRoom[Random.Range(0, playerInRoom.Length)];

			m_king.SetKing(true);
			PhotonNetwork.CurrentRoom.SetKingHealth(KingMaxHealth);
			Teams.AssignTo(m_king, Team.Aggressive);
		}

		/// <summary>
		/// Team Management
		/// </summary>
		public override void HandlePlayer()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				SetKing();
				foreach (var player in PhotonNetwork.PlayerList)
				{
					Assign(player);
				}
			}
		}

		/// <summary>
		/// GameMode specific Spawning.
		/// </summary>
		/// <param name="player"></param>
		public override void SpawnPlayer(Player player)
		{
			SpawnEvents.RespawnRandomSpawnNode(player);
		}

		/// <summary>
		/// Respawn for Player that are not the King.
		/// </summary>
		/// <returns></returns>
		public override bool AllowRespawn()
		{
			return true;
		}

		/// <summary>Only retrieve cached values,will not trigger room properties.</summary>
		/// <returns></returns>
		public float GetKingsHealth()
		{
			var health = PhotonNetwork.CurrentRoom.GetKingHealth();
			return health;
		}

		/// <summary>
		/// If Kings Health is 0, king is dead.
		/// </summary>
		protected override bool WinCondition()
		{
			if (m_king != null && !m_king.IsAlive())
			{
				PhotonNetwork.CurrentRoom.SetKingHealth(0);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Win Text, Leading Player or Team.
		/// </summary>
		/// <returns></returns>
		public override string GetLeading()
		{
			return WinCondition() ? "King lost!" : "King won!";
		}

		/// <summary>
		/// Make Kings health available for other. 
		/// </summary>
		/// <returns></returns>
		protected override string[] ConfigStats()
		{
			Stats[3] = GetKingsHealth().ToString();
			return Stats;
		}
	}
}