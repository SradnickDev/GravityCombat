using ExitGames.Client.Photon;
using Network.Extensions;
using Photon.Pun;
using UI.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class Matchmaking : MonoBehaviourPunCallbacks
	{
		[Header("Matchmaking")] [SerializeField]
		private LogInfo LogInfo = null;

		[SerializeField] private SceneHandling.SceneContainer WaitingRoom = null;
		[SerializeField] private Text Timer = null;

		private Hashtable m_expectedProperties = new Hashtable();
		private string m_pickedMode;
		private bool m_lookingForMatch = false;
		private float m_timer = 0.0f;

		public void OnPickMode(string mode)
		{
			m_pickedMode = mode;
			m_timer = 0;
			StartSearch();
		}

		private void StartSearch()
		{
			if (PhotonNetwork.IsConnectedAndReady)
			{
				LogInfo.Write(LogInfo.LogType.Success, "Searching for Game");
				m_expectedProperties = new Hashtable() {{RoomProperties.GameMode, m_pickedMode}};
				m_lookingForMatch = true;
			}
			else
			{
				LogInfo.Write(LogInfo.LogType.Warning, "Not Connected.");
			}
		}

		private void Update()
		{
			LookingForMatch();
		}

		/// <summary>
		/// While a timer is running, looking every 5 secs for a Room with the Expected Properties.
		/// </summary>
		private void LookingForMatch()
		{
			if (!m_lookingForMatch) return;

			m_timer += Time.deltaTime;

			var (minutes, seconds) = ToMinSec(m_timer);
			Timer.text = minutes + " : " + seconds;

			if (Mathf.RoundToInt(m_timer) % 5 == 0 && PhotonNetwork.CountOfPlayers >= 2)
			{
				PhotonNetwork.JoinRandomRoom(m_expectedProperties, 6);
			}
		}

		private (string minutes, string seconds) ToMinSec(float time)
		{
			var minutes = Mathf.Floor(time / 60).ToString("00");
			var seconds = (time % 60).ToString("00");
			return (minutes, seconds);
		}

		//Called form Photons API after successfully Joint a Room
		public override void OnJoinedRoom()
		{
			if (!m_lookingForMatch) return;

			LogInfo.Write(LogInfo.LogType.Success, "Connected!");
			LoadingScreen.LoadScene(WaitingRoom.SceneIndex);
		}
	}
}