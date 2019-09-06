using Network.Extensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network.Match
{
	/// <summary>
	/// Should be placed on the Player GameObject that will be instantiated if player joined a room.
	/// </summary>
	public class ReadyOnSceneLoaded : MonoBehaviour
	{
		public void OnEnable()
		{
			//Register OnSceneLoaded to an Unity SceneManger Event
			//is called when the Scene is loaded
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void OnDisable()
		{
			//unregister when Disabled or Destroyed or scene changed
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			//Unity Callback
			//enable Message Queue, to receive messages from photon
			//client is ready to get spawn infos
			PhotonNetwork.IsMessageQueueRunning = true;
			PhotonNetwork.LocalPlayer.SetReady(true);
		}
	}
}