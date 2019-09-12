using Cam;
using ExitGames.Client.Photon;
using Network.Match;
using Photon.Pun;
using Photon.Realtime;
using PlayerBehaviour.Model;
using UnityEngine;

namespace Network
{
	/// <summary>
	/// Reacts to Spawn event, creates Player Object and setup components.
	/// </summary>
	public class PlayerSetup : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		[SerializeField] private GameObject PlayerPrefab = null;
		[SerializeField] private Camera Camera = null;
		[SerializeField] private InterpolatedCamera InterpolatedCamera = null;
		[SerializeField] private MatchRespawn MatchRespawn = null;

		public void OnEvent(EventData photonEvent)
		{
			switch (photonEvent.Code)
			{
				case SpawnEventProperties.Spawn:
					var data = (object[]) photonEvent.CustomData;
					var position = (Vector3) data[0];
					var rotation = (Quaternion) data[1];

					CreatePlayer(position, rotation);
					break;
			}
		}

		#region CreatePlayer

		private void CreatePlayer(Vector3 spawnPosition, Quaternion spawnRotation)
		{
			var playerObject = PhotonNetwork.Instantiate(PlayerPrefab.name,
														new Vector3(spawnPosition.x, spawnPosition.y, 0),
														spawnRotation, 0);

			SetUpComponents(playerObject);
		}

		private void SetUpComponents(GameObject player)
		{
			var healthModel = player.GetComponent<PlayerHealthModel>();
			var playerMovement = player.GetComponent<PlayerMovementModel>();

			healthModel.Camera = Camera;

			MatchRespawn.Init(healthModel);

			playerMovement.Camera = Camera;
			InterpolatedCamera.Target = player.transform;
		}

		#endregion CreatePlayer
	}
}