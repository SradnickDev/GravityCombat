using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;

namespace Network.Gamemode
{
	public sealed class LastManStanding : GameModeBase
	{
		public LastManStanding()
		{
			Stats[2] = "0";
		}

		public override void Start()
		{
			base.Start();

			var players = PhotonNetwork.PlayerList;
		}

		protected override void Assign(Player player)
		{
			Teams.AssignTo(player, Team.Aggressive);
		}

		/// <summary>Returns cached Value.</summary>
		private int PlayersLeft()
		{
			var playersLeft = 0;

			foreach (var player in PhotonNetwork.PlayerList)
			{
				if (player.IsAlive())
				{
					playersLeft++;
				}
			}

			return playersLeft;
		}

		public override void SpawnPlayer(Player player)
		{
			SpawnEvents.RespawnRandomSpawnNode(player);
		}

		public override bool AllowRespawn()
		{
			return false;
		}

		/// <summary>
		/// if one player left.
		/// </summary>
		protected override bool WinCondition()
		{
			return PhotonNetwork.CurrentRoom.PlayerCount > 1 && PlayersLeft() == 1;
		}

		public override string GetLeading()
		{
			Player lead = null;
			foreach (var player in PhotonNetwork.PlayerList)
			{
				if (player.IsAlive() || lead == null)
				{
					lead = player;
				}
			}

			return lead.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber ? "Victory" : "Defeat";
		}

		protected override string[] ConfigStats()
		{
			Stats[2] = PlayersLeft().ToString();
			return Stats;
		}
	}
}