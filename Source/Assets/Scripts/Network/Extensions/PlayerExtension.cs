using Network.Gamemode;
using Photon.Pun;
using Photon.Realtime;

namespace Network.Extensions
{
	/// <summary>
	/// Player Extension to wrap up access to player's Custom properties
	/// </summary>
	public static class PlayerExtensions
	{
		#region PlayerKills

		/// <summary>
		/// Assign Score to Player Properties.Overrides the current Score.
		/// </summary>
		/// <param name="player">Target Photon Player</param>
		/// <param name="value">Score</param>
		public static void SetKill(this Player player, int value)
		{
			player.SetPropertyValue(PlayerProperties.Kills, value);
		}

		/// <summary>
		/// Add Score to PLayer Properties.
		/// </summary>
		public static void AddKill(this Player player, int value)
		{
			player.AddValueToProperty(PlayerProperties.Kills, value);
		}

		/// <summary>Get the current Player Score.</summary>
		public static int GetKills(this Player player)
		{
			return player.GetPropertyValue(PlayerProperties.Kills, 0);
		}

		#endregion

		#region PlayerAssist

		public static void SetAssist(this Player player, int value)
		{
			player.SetPropertyValue(PlayerProperties.Assist, value);
		}

		public static void AddAssist(this Player player, int value)
		{
			player.AddValueToProperty(PlayerProperties.Assist, value);
		}

		public static int GetAssists(this Player player)
		{
			return player.GetPropertyValue(PlayerProperties.Assist, 0);
		}

		#endregion

		#region PlayerDeaths

		public static void SetDeaths(this Player player, int value)
		{
			player.SetPropertyValue(PlayerProperties.Deaths, value);
		}

		public static void AddDeath(this Player player, int value)
		{
			player.AddValueToProperty(PlayerProperties.Deaths, value);
		}

		public static int GetDeaths(this Player player)
		{
			return player.GetPropertyValue(PlayerProperties.Deaths, 0);
		}

		#endregion

		#region PlayerScore

		public static void SetScore(this Player player, int value)
		{
			player.SetPropertyValue(PlayerProperties.Score, value);
		}

		public static void AddScore(this Player player, int value)
		{
			player.AddValueToProperty(PlayerProperties.Score, value);
		}

		public static int GetScore(this Player player)
		{
			return player.GetPropertyValue(PlayerProperties.Score, 0);
		}

		#endregion

		#region PlayerReadyState

		/// <summary>Change PhotonPlayer Ready State.</summary>
		public static void SetReady(this Player player, bool value)
		{
			player.SetPropertyValue(PlayerProperties.Ready, value);
		}

		/// <summary>Get PhotonPlayer Ready State, if null its not set.</summary>
		public static bool IsReady(this Player player)
		{
			if (player.CustomProperties.TryGetValue(PlayerProperties.Ready, out var activState))
			{
				return (bool) activState;
			}

			return false;
		}

		#endregion

		#region Teams

		/// <summary>Players Team if assign, else Team.None</summary>
		/// <returns>Team.none if no team was found or added.</returns>
		public static Team GetTeam(this Player player)
		{
			return (Team) player.GetPropertyValue(PlayerProperties.Team, Team.None);
		}

		/// <summary>Switch that player's team to the one you assign.</summary>
		/// <param name="player">Target Player</param>
		/// <param name="team">Target Team</param>
		public static void SetTeam(this Player player, Team team)
		{
			player.SetPropertyValue(PlayerProperties.Team, team);
		}

		/// <summary>
		/// True if in same Team , false if not or Team Aggressive. 
		/// Compares targetPlayer with localPlayer.
		/// </summary>
		/// <param name="targetPlayer">Target Player.</param>
		/// <returns></returns>
		public static bool IsFriendly(this Player targetPlayer)
		{
			var targetTeam = targetPlayer.GetTeam();
			if (targetTeam == Team.Aggressive) return false;
			return PhotonNetwork.LocalPlayer.GetTeam() == targetTeam;
		}

		#endregion

		#region Reset

		/// <summary>
		/// Deletes all Properties.
		/// </summary>
		/// <param name="player">Target Player</param>
		public static void ResetProperties(this Player player)
		{
			player.SetKill(0);
			player.SetScore(0);
			player.SetAssist(0);
			player.SetDeaths(0);
		}

		#endregion

		#region IsAlive

		/// <summary>
		/// Can be used to stored current Player state.
		/// e.g Player dies, set prop to false.
		/// </summary>
		/// <param name="player">Target Player</param>
		/// <returns></returns>
		public static bool IsAlive(this Player player)
		{
			return (bool) player.GetPropertyValue(PlayerProperties.Alive, false);
		}

		/// <summary>
		/// Change current Players state.
		/// </summary>
		/// <param name="player">Target Player</param>
		/// <param name="alive">True/False == Alive/Dead</param>
		public static void SetAlive(this Player player, bool alive)
		{
			player.SetPropertyValue(PlayerProperties.Alive, alive);
		}

		#endregion

		#region King

		/// <summary>
		/// Used for special case like Game Mode Kill the king.
		/// So every client can ask each client if he is the King or not.
		/// </summary>
		/// <param name="player">Target PLayer</param>
		/// <returns></returns>
		public static bool IsKing(this Player player)
		{
			var currentGameMode = PhotonNetwork.CurrentRoom.GetGameMode();
			if (currentGameMode is KillTheKing)
			{
				return (bool) player.GetPropertyValue(PlayerProperties.King, false);
			}

			return false;
		}

		/// <summary>
		/// If not set it is by default false.
		/// </summary>
		/// <param name="player">Target Player</param>
		/// <param name="isKing">true or false</param>
		public static void SetKing(this Player player, bool isKing)
		{
			player.SetPropertyValue(PlayerProperties.King, isKing);
		}

		#endregion
	}
}