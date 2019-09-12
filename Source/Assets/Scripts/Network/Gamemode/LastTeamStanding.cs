using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;

namespace Network.Gamemode
{
	public sealed class LastTeamStanding : GameModeBase
	{
		public LastTeamStanding()
		{
			Stats[0] = "0";
			Stats[1] = "0";
		}
		protected override void Assign(Player player)
		{
			if (player.GetTeam() == Team.None || player.GetTeam() == Team.Aggressive)
			{
				Teams.AssignToSmallest(player);
			}
		}

		private int FriendlyPlayersAlive()
		{
			return GetTeamSize(Teams.GetLocalTeam());
		}

		private int EnemyPlayersAlive()
		{
			return GetTeamSize(Teams.GetEnemyTeam());
		}

		public override void SpawnPlayer(Player player)
		{
			SpawnEvents.TeamBasedRespawn(player.GetTeam(), player);
		}

		public override bool AllowRespawn()
		{
			return false;
		}

		/// <summary>
		/// Counts player from specific Team.
		/// </summary>
		/// <param name="team">Which Team</param>
		/// <returns>Count of player with Alive propery to true</returns>
		private int GetTeamSize(Team team)
		{
			var aliveCount = 0;

			foreach (var player in PhotonNetwork.PlayerList)
			{
				if (player.IsAlive() && player.GetTeam() == team)
				{
					aliveCount++;
				}
			}

			return aliveCount;
		}

		/// <summary>
		/// If no one of a Team is alive, other Team wins.
		/// </summary>
		protected override bool WinCondition()
		{
			var pCount = (int) PhotonNetwork.CurrentRoom.PlayerCount;
			return FriendlyPlayersAlive() == 0 && pCount > 1 || EnemyPlayersAlive() == 0 && pCount > 1;
		}

		public override string GetLeading()
		{
			return FriendlyPlayersAlive() > EnemyPlayersAlive() ? "Victory" : "Defeat";
		}

		protected override string[] ConfigStats()
		{
			Stats[0] = FriendlyPlayersAlive().ToString();
			Stats[1] = EnemyPlayersAlive().ToString();
			return Stats;
		}
	}
}