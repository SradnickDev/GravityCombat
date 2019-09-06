using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;

namespace Network.Gamemode
{
	public sealed class Deathmatch : GameModeBase
	{
		public Deathmatch()
		{
			Stats[0] = "0";
			Stats[1] = "0";
		}

		/// <summary>
		/// Assign Player to team Aggressive => which is for Deathmatch
		/// </summary>
		/// <param name="player">Player to join a Team</param>
		protected override void Assign(Player player)
		{
			Teams.AssignTo(player, Team.Aggressive);
		}

		public override void SpawnPlayer(Player player)
		{
			SpawnEvents.TeamBasedRespawn(Team.Aggressive, player);
		}

		public override bool AllowRespawn()
		{
			return true;
		}

		/// <summary>
		/// Highest score from single Player
		/// </summary>
		/// <returns>Highest score</returns>
		private int GetHighestScore()
		{
			var playerList = PhotonNetwork.PlayerList;
			var score = 0;

			foreach (var player in playerList)
			{
				var temp = player.GetScore();
				if (temp > score)
				{
					score = temp;
				}
			}

			return score;
		}

		private int GetLocalScore()
		{
			return PhotonNetwork.LocalPlayer.GetScore();
		}

		public override string GetLeading()
		{
			var playerList = PhotonNetwork.PlayerList;
			var score = 0;
			Player lead = null;

			foreach (var player in playerList)
			{
				var temp = player.GetScore();
				if (temp > score || lead == null)
				{
					score = temp;
					lead = player;
				}
			}

			return lead.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber ? "Victory" : "Defeat";
		}

		protected override string[] ConfigStats()
		{
			Stats[0] = GetLocalScore().ToString();
			Stats[1] = GetHighestScore().ToString();
			return Stats;
		}
	}
}