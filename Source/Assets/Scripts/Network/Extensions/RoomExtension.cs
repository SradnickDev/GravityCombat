using System.Diagnostics;
using Network.Gamemode;
using Photon.Realtime;

namespace Network.Extensions
{
	/// <summary>
	/// Player Extension to wrap up access to player's Custom properties
	/// Extension for PhotonPlayer class to wrap up access to the player's custom properties.
	/// </summary>
	public static class RoomExtension
	{
		#region Ping

		/// <summary>Store Ping in Room Properties.</summary>
		/// <param name="room"></param>
		/// <param name="value"></param>s
		public static void SetPing(this Room room, int value)
		{
			room.SetPropertyValue(RoomProperties.Ping, value);
		}

		/// <summary>Get Ping from Room Propertiess</summary>
		/// <param name="room"></param>
		/// <returns></returns>
		public static int GetPing(this Room room)
		{
			var ping = room.GetPropertyValue(RoomProperties.Ping, 0);
			return ping;
		}

		#endregion

		#region Map

		/// <summary>Store picked Map in Room Properties.</summary>
		/// <param name="room"></param>
		/// <param name="mapName"></param>
		public static void SetMap(this Room room, string mapName)
		{
			room.SetPropertyValue(RoomProperties.Map, mapName);
		}

		/// <summary>Get Map Name from Room Properties.</summary>
		/// <param name="room"></param>
		/// <returns></returns>
		public static string GetMap(this Room room)
		{
			var mapName = room.GetPropertyValue(RoomProperties.Map, "null");
			return mapName;
		}

		#endregion

		#region GameMode

		/// <summary>
		/// Store GameMode in Room Properties.
		/// Should be set by Room creation.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="name"></param>
		public static void SetGameMode(this Room room, string name)
		{
			room.SetPropertyValue(RoomProperties.GameMode, name);
		}

		/// <summary>
		/// Checks Room Properties for stored string.
		///	Creates a instance and returns it as GameMode.
		/// </summary>
		/// <param name="room">Stored Mode instance casted into GameMode</param>
		/// <returns></returns>
		public static GameModeBase GetGameMode(this Room room)
		{
			var mode = room.GetPropertyValue(RoomProperties.GameMode, string.Empty);
			Debug.Assert(mode != null, nameof(mode) + " != null");
			
			var type = $"{typeof(GameModeBase).Namespace}.{mode}";
			return (GameModeBase) System.Activator.CreateInstance(System.Type.GetType(type));
		}

		#endregion

		#region Rounds

		/// <summary>
		/// Can be used to set an init amount of rounds.
		/// </summary>
		/// <param name="room">Current Room</param>
		/// <param name="number">Rounds</param>
		public static void SetRounds(this Room room, int number)
		{
			room.SetPropertyValue(RoomProperties.Round, number);
		}

		/// <summary>
		/// Current amount of rounds.
		/// </summary>
		/// <param name="room">current room</param>
		/// <returns>Amount of rounds in room</returns>
		public static int GetRounds(this Room room)
		{
			var retVal = room.GetPropertyValue(RoomProperties.Round, 0);
			return retVal;
		}

		#endregion

		#region Match Time

		/// <summary>
		/// Time for a Match.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="value"></param>
		public static void SetMatchTime(this Room room, float value)
		{
			room.SetPropertyValue(RoomProperties.Time, value);
		}

		/// <summary>
		/// Left Time for current running Match.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		public static float GetMatchTime(this Room room)
		{
			var retVal = room.GetPropertyValue(RoomProperties.Time, 0.0f);
			return retVal;
		}

		#endregion

		#region Weapon Spawn Time

		/// <summary>
		/// To set Spawn Time.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="time"></param>
		public static void SetWeaponSpawnTime(this Room room, int time)
		{
			room.SetPropertyValue(RoomProperties.WeaponSpawnTime, time);
		}

		/// <summary>
		/// Spawn Time from Room properties.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		public static int GetWeaponSpawnTime(this Room room)
		{
			var retVal = room.GetPropertyValue(RoomProperties.WeaponSpawnTime, 0);
			return retVal;
		}

		#endregion

		#region KingHealth

		/// <summary>Store King Health,only in GameMode KillTheKing.</summary>
		/// <param name="room"></param>
		/// <param name="value"></param>s
		public static void SetKingHealth(this Room room, float value)
		{
			room.SetPropertyValue(RoomProperties.KingHealth, value);
		}

		/// <summary>
		/// Get health from king.
		/// By default its 0.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		public static float GetKingHealth(this Room room)
		{
			var health = room.GetPropertyValue(RoomProperties.KingHealth, 0.0f);
			return health;
		}

		public static bool KingAlive(this Room player)
		{
			return (bool) player.GetPropertyValue(RoomProperties.KingAlive, false);
		}

		public static void KingAlive(this Room player, bool isKing)
		{
			player.SetPropertyValue(RoomProperties.KingAlive, isKing);
		}

		#endregion
	}
}