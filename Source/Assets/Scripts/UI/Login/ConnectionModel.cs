using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Login
{
	public class ConnectionModel : MonoBehaviourPunCallbacks
	{
		public UnityEvent OnSuccessfulConnect;

		private void Start()
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
		/// Fixed Region, Scene Sync and App Version Settings
		/// </summary>
		private void SetDefaultSettings()
		{
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
			PhotonNetwork.AutomaticallySyncScene = false;
			PhotonNetwork.GameVersion = Application.version;
		}

		public void Authentication(string username, string password)
		{
			PhotonNetwork.AuthValues = new AuthenticationValues
			{
				AuthType = CustomAuthenticationType.Custom
			};

			PhotonNetwork.AuthValues.AddAuthParameter("usernamePost", username);
			PhotonNetwork.AuthValues.AddAuthParameter("passwordPost", password);

			//Connect(username);
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