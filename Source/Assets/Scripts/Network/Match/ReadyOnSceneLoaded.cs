using Network.Extensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network.Match
{
	/// <summary>
	/// Should be placed on the Player GameObject that will be instantiated if client joined a room.
	/// </summary>
	public class ReadyOnSceneLoaded : MonoBehaviour
	{
		public void OnEnable()
		{
			//Register OnSceneLoaded to an Unity SceneManger Event
			//is called when the Scene is loaded
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}
		

		public void OnDisable()
		{
			//unregister when Disabled or Destroyed or scene changed
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnActiveSceneChanged(Scene arg0, Scene arg1)
		{
			//this client is not ready till the new scene is loaded
			PhotonNetwork.LocalPlayer.SetReady(false);
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