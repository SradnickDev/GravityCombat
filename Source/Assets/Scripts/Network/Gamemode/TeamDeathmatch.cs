using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;

namespace Network.Gamemode
{
	public sealed class TeamDeathmatch : GameModeBase
	{
		public TeamDeathmatch()
		{
			Stats[0] = "0";
			Stats[1] = "0";
		}

		protected override void Assign(Player player)
		{
			Teams.AssignToSmallest(player);
		}

		/// <summary>
		/// Local Team , cached values(score).
		/// </summary>
		/// <returns></returns>
		private float GetLocalTeamScore()
		{
			var localTeam = Teams.GetLocalTeam();
			return GetTeamScore(localTeam);
		}

		/// <summary>
		/// Enemy Team, cached values(score).
		/// </summary>
		/// <returns></returns>
		private float GetEnemyTeamScore()
		{
			var enemyTeam = Teams.GetEnemyTeam();
			return GetTeamScore(enemyTeam);
		}

		private float GetTeamScore(Team team)
		{
			var score = 0.0f;

			foreach (var player in PhotonNetwork.PlayerList)
			{
				if (player.GetTeam() == team)
				{
					score += player.GetScore();
				}
			}

			return score;
		}

		public override void SpawnPlayer(Player player)
		{
			SpawnEvents.TeamBasedRespawn(player.GetTeam(), player);
		}

		public override bool AllowRespawn()
		{
			return true;
		}

		public override string GetLeading()
		{
			return GetLocalTeamScore() > GetEnemyTeamScore() ? "Victory" : "Defeat";
		}

		protected override string[] ConfigStats()
		{
			Stats[0] = GetLocalTeamScore().ToString();
			Stats[1] = GetEnemyTeamScore().ToString();
			return Stats;
		}
	}
}