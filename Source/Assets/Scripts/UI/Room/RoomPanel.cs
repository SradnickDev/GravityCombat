using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Room
{
	/// <summary>
	/// Visualisation of an open Room and used to Join.
	/// </summary>
	public class RoomPanel : MonoBehaviourPunCallbacks
	{
		[SerializeField] private Text RoomNameLabel = null;
		[SerializeField] private Text MapNameLabel = null;
		[SerializeField] private Text GameModeLabel = null;
		[SerializeField] private Text PlayerCountLabel = null;
		[SerializeField] private Button Join = null;
		[SerializeField] private Text PingLabel = null;

		public void SetRoom(RoomInfo room)
		{
			var map = room.CustomProperties[RoomProperties.Map].ToString();
			var modeName = room.CustomProperties[RoomProperties.GameMode].ToString();
			RefreshPanelLabels(room, map, modeName);
		}

		/// <summary>
		/// To Join this Room
		/// </summary>
		/// <param name="action"></param>
		public void AddClickListener(UnityAction action)
		{
			Join.onClick.AddListener(action);
		}

		/// <summary>
		/// Updates Information which will be displayed
		/// </summary>
		public void UpdatePanel(RoomInfo room)
		{
			var pingValue = 0;
			if (room.CustomProperties.TryGetValue(RoomProperties.Ping, out var value))
			{
				pingValue = (int) value;
			}

			PingLabel.text = pingValue.ToString();
			PlayerCountLabel.text = $"{room.PlayerCount} / {room.MaxPlayers}";
		}

		private void RefreshPanelLabels(RoomInfo room, string map, string modeName)
		{
			RoomNameLabel.text = room.Name;
			MapNameLabel.text = map;
			GameModeLabel.text = modeName;
			PlayerCountLabel.text = $"{room.PlayerCount} / {room.MaxPlayers}";
		}
	}
}