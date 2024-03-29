using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

namespace Network.Extensions
{
	//None as a Fallback, Back and White for Team Deathmatch, Aggressive for Deathmatch
	public enum Team : byte
	{
		None,
		Black,
		White,
		Aggressive
	};

	public class Teams
	{
		/// <summary>
		/// Current Team size.
		/// </summary>
		/// <param name="team">From which Team</param>
		/// <returns>Team Size</returns>
		public int GetSize(Team team)
		{
			var playersInRoom = PhotonNetwork.PlayerList;
			var teamSize = playersInRoom.Count(player => player.GetTeam() == team);
			return teamSize;
		}

		/// <summary>
		/// Return cached team from local player.
		/// </summary>
		/// <returns></returns>
		public Team GetLocalTeam()
		{
			return PhotonNetwork.LocalPlayer.GetTeam();
		}

		/// <summary>
		/// None if no Enemy Team is available.
		/// </summary>
		/// <returns></returns>
		public Team GetEnemyTeam()
		{
			switch (GetLocalTeam())
			{
				case Team.Black:
					return Team.White;
				case Team.White:
					return Team.Black;
				default:
					return Team.None;
			}
		}

		/// <summary>
		/// Returns a all Player in a given Team
		/// </summary>
		/// <param name="team">From which Team</param>
		/// <returns>Player List</returns>
		public IEnumerable<Player> Players(Team team)
		{
			return PhotonNetwork.PlayerList.Where(player => player.GetTeam() == team);
		}

		/// <summary>
		/// Set Player to the Team with the smallest size.
		/// </summary>
		/// <param name="player">Which Player.</param>
		public void AssignToSmallest(Player player)
		{
			var blackSize = GetSize(Team.Black);
			var whiteSize = GetSize(Team.White);

			var smallerTeam = blackSize >= whiteSize ? Team.White : Team.Black;

			AssignTo(player, smallerTeam);
		}

		/// <summary>Specify which player should join which Team.</summary>
		public void AssignTo(Player player, Team team)
		{
			player.SetTeam(team);
		}
	}
}