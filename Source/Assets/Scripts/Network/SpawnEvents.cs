using ExitGames.Client.Photon;
using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Network
{
	public class SpawnEventProperties
	{
		public const byte Spawn = 20;
	}

	public static class SpawnEvents
	{
		#region Events

		public delegate Transform OnSyncSpawnNode(Team team);

		public static event OnSyncSpawnNode SyncSpawnNodeEvent;

		public delegate Transform OnTeamBasedRespawn(Team team);

		public static event OnTeamBasedRespawn TeamBasedRespawnEvent;

		public delegate Transform OnRespawnRandom();

		public static event OnRespawnRandom RespawnRandomEvent;

		#endregion

		/// <summary>
		/// Using Photons Raise Event to send Spawn Event with Position and Rotation to Target Player
		/// </summary>
		/// <param name="team">To Specific which Spawn Nodes can be used.</param>
		/// <param name="player">Target Player</param>
		public static void SyncSpawnNode(Team team, Player player)
		{
			if (SyncSpawnNodeEvent != null)
			{
				var spawnNode = SyncSpawnNodeEvent(team);

				PhotonNetwork.RaiseEvent(SpawnEventProperties.Spawn,
										new object[] {spawnNode.position, spawnNode.rotation},
										new RaiseEventOptions
										{
											TargetActors = new int[] {player.ActorNumber},
											CachingOption = EventCaching.AddToRoomCache
										}, SendOptions.SendReliable);
			}
		}

		/// <summary>
		/// Using Photons Raise Event to send Spawn Event with Position and Rotation to Target Player
		/// </summary>
		/// <param name="team">To Specific which Spawn Nodes can be used.</param>
		/// <param name="player">Target Player</param>
		public static void TeamBasedRespawn(Team team, Player player)
		{
			if (SyncSpawnNodeEvent != null)
			{
				if (TeamBasedRespawnEvent != null)
				{
					var spawnNode = TeamBasedRespawnEvent(team);

					PhotonNetwork.RaiseEvent(SpawnEventProperties.Spawn,
											new object[] {spawnNode.position, spawnNode.rotation},
											new RaiseEventOptions
											{
												TargetActors = new int[] {player.ActorNumber},
												CachingOption = EventCaching.AddToRoomCache
											}, SendOptions.SendReliable);
				}
			}
		}

		/// <summary>
		/// Using Photons Raise Event to send Spawn Event with Position and Rotation to Target Player.
		/// Randomly Spawn node will be picked.
		/// </summary>
		/// <param name="player">Target Player</param>
		public static void RespawnRandomSpawnNode(Player player)
		{
			if (SyncSpawnNodeEvent != null)
			{
				var spawnPoint = new Vector3(0, 0, 0);
				var spawnRotation = Quaternion.identity;

				var spawnNode = RespawnRandomEvent();
				Debug.Assert(spawnNode != null, "Missing Spawn Node");

				if (spawnNode != null)
				{
					spawnPoint = spawnNode.position;
					spawnRotation = spawnNode.rotation;
				}


				PhotonNetwork.RaiseEvent(SpawnEventProperties.Spawn,
										new object[] {spawnPoint, spawnRotation},
										new RaiseEventOptions
										{
											TargetActors = new int[] {player.ActorNumber},
											CachingOption = EventCaching.AddToRoomCache
										}, SendOptions.SendReliable);
			}
		}
	}
}