using Photon.Pun;
using UnityEngine;

namespace Utilities
{
	/// <summary>
	/// Contains no relevant Game code.
	/// Only purposes is to gain some information to debug the Client.
	/// </summary>
	public class PropertyView : MonoBehaviour
	{
		private bool m_enabled = false;
		private GUIStyle m_style = new GUIStyle();

		private void Start()
		{
			m_style.normal.textColor = Color.red;
			m_style.fontSize = 25;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				m_enabled = !m_enabled;
			}
		}

		private void OnGUI()
		{
			if (!m_enabled) return;
			var lineHeight = 30;
			var idx = 1;

			GUI.Label(new Rect(10, 0, 100, 20), PhotonNetwork.CloudRegion, m_style);
			GUI.Label(new Rect(10, lineHeight * idx, 100, 20), "State : " + PhotonNetwork.NetworkClientState, m_style);
			idx++;
			GUI.Label(new Rect(10, lineHeight * idx, 100, 20), $" In Room :{PhotonNetwork.InRoom}", m_style);
			idx++;
			GUI.Label(new Rect(10, lineHeight * idx, 100, 20), $" In Lobby :{PhotonNetwork.InLobby}", m_style);
			idx++;
			GUI.Label(new Rect(10, lineHeight * idx, 100, 20),
					$"Lobby Infos: {PhotonNetwork.CurrentLobby.Name} , {PhotonNetwork.CurrentLobby.Type}", m_style);
			idx++;
			GUI.Label(new Rect(10, lineHeight * idx, 100, 20),
					$"Connected and Ready : {PhotonNetwork.IsConnectedAndReady}",
					m_style);
			idx++;
			GUI.Label(new Rect(10, lineHeight * idx, 100, 20), $"Count of Players: {PhotonNetwork.CountOfPlayers}",
					m_style);
			idx++;

			if (PhotonNetwork.LocalPlayer != null && PhotonNetwork.InRoom)
			{
				foreach (var property in PhotonNetwork.LocalPlayer.CustomProperties)
				{
					var prop = $"Player Props : {property.Key} = {property.Value}";
					GUI.Label(new Rect(10, lineHeight * idx, 100, 20), prop, m_style);
					idx++;
				}

				foreach (var property in PhotonNetwork.CurrentRoom.CustomProperties)
				{
					var prop = $"Room Props : {property.Key} = {property.Value}";
					GUI.Label(new Rect(10, lineHeight * idx, 100, 20), prop, m_style);
					idx++;
				}
			}
		}
	}
}