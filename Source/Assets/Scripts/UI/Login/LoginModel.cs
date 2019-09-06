using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UI.Flex;
using UI.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Login
{
	/// <summary>
	/// Used to connect to Photon Cloud Service. On first start and after leaving a match.
	/// Gives the player information about the connection state. Using edge cases to reconnect automatically.
	/// </summary>
	[RequireComponent(typeof(ConnectionModel))]
	public class LoginModel : FlexScreen
	{
		public const string NicknamePref = "Nickname";

		[SerializeField] private InputField NameField = null;
		[SerializeField] private Button ConnectBtn = null;
		[SerializeField] private ConnectionModel ConnectionModel = null;
		[SerializeField] private LogInfo ConnectionInfo = null;

		private string m_nickname
		{
			get { return PlayerPrefs.GetString(NicknamePref); }
			set { PlayerPrefs.SetString(NicknamePref, value); }
		}

		private bool m_HasNickname
		{
			get { return PlayerPrefs.HasKey(NicknamePref); }
		}

		private void Awake()
		{
			if (!m_HasNickname)
			{
				NameField.gameObject.SetActive(true);
				ConnectBtn.gameObject.SetActive(true);
			}
			else
			{
				Connect();
			}
		}

		/// <summary>
		/// Used to connect to Photon Cloud Service.
		/// </summary>
		public void Connect()
		{
			if (!m_HasNickname)
			{
				m_nickname = NameField.text;
			}


			switch (PhotonNetwork.NetworkClientState)
			{
				case ClientState.JoiningLobby:
					ConnectionModel.OnConnectedToMaster();
					ConnectionInfo.Write(LogInfo.LogType.Success, "joining lobby...");
					ConnectionModel.OnSuccessfulConnect?.Invoke();

					break;
				case ClientState.JoinedLobby:
					ConnectionInfo.Write(LogInfo.LogType.Success, "joined lobby...");
					break;

				case ClientState.ConnectingToMasterServer:
				case ClientState.PeerCreated:
				case ClientState.DisconnectingFromGameServer:
				case ClientState.Disconnected:
					ConnectionModel.Connect(m_nickname);
					ConnectionInfo.Write(LogInfo.LogType.Success, "connecting...");
					break;

				default:
					var msges = new[]
					{
						"Spinning up the hamster...",
						"Hatching an egg...",
						"Getting some dream letters...",
						"Throwing some stones..."
					};
					var msg = msges[Random.Range(0, msges.Length - 1)];

					ConnectionInfo.Write(LogInfo.LogType.Success, msg);
					break;
			}

			StartCoroutine(CheckConnection());
		}

		/// <summary>
		/// While not connected and ready try to connect again. 
		/// </summary>
		private IEnumerator CheckConnection()
		{
			yield return new WaitForSeconds(5);
			if (!PhotonNetwork.IsConnectedAndReady)
			{
				Connect();
			}
		}
	}
}