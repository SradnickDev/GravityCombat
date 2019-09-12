using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Login
{
	public class ConnectionModel : MonoBehaviourPunCallbacks
	{
		public UnityEvent OnSuccessfulConnect;

		public void Setup()
		{
			SetSendRate();
			SetDefaultSettings();
		}

		/// <summary>
		/// Specify send rate.
		/// </summary>
		private void SetSendRate()
		{
			PhotonNetwork.SendRate = 12;
			PhotonNetwork.SerializationRate = 12;
		}

		/// <summary>
		/// Fixed Region, Scene Sync and App Version Settings.
		/// AppVersion should set before connecting.
		/// </summary>
		private void SetDefaultSettings()
		{
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
			PhotonNetwork.AutomaticallySyncScene = false;
			PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = Application.version;
		}

		/// <summary>
		/// Connect to Photon with settings.
		/// </summary>
		/// <returns>True if connecting is possible otherwise false.</returns>
		public bool Connect(string nickName)
		{
			PhotonNetwork.NickName = nickName;
			return PhotonNetwork.ConnectUsingSettings();
		}

		/// <summary>
		/// Photon Callback.
		/// </summary>
		public override void OnConnectedToMaster()
		{
			var typedLobby = TypedLobby.Default;
			typedLobby.Name = "GC";
			PhotonNetwork.JoinLobby(typedLobby);
		}

		/// <summary>
		/// Photon Callback.
		/// </summary>
		public override void OnJoinedLobby()
		{
			OnSuccessfulConnect?.Invoke();
		}
	}
}