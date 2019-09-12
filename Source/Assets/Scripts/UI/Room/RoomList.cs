using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UI.Flex;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Room
{
	public class RoomList : MonoBehaviourPunCallbacks, IFlexScreen
	{
		public UnityEvent OnRoomSelected;

		[Header("Reference")] [SerializeField] private GameObject RoomPanel = null;

		[SerializeField] private Transform Content = null;

		[Header("Map")] [SerializeField] SceneHandling.SceneContainer WaitingRoom = null;

		private List<RoomInfo> m_cachedRoomList = new List<RoomInfo>();
		private Dictionary<string, RoomPanel> m_roomPanelList = new Dictionary<string, RoomPanel>();
		private DoubleInput m_doubleInput = new DoubleInput(1);
		private bool m_roomSelected = false;

		public void Open()
		{
			gameObject.SetActive(true);
		}

		public void Close()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Fake refresh.
		/// </summary>
		public void Refresh()
		{
			OnRoomListUpdate(m_cachedRoomList);
		}

		/// <summary>
		/// Photon Callback
		/// Used to create , update , delete RoomPanels.
		/// </summary>
		/// <param name="roomList"></param>
		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			foreach (var entry in roomList)
			{
				if (m_roomPanelList.ContainsKey(entry.Name))
				{
					if (entry.RemovedFromList)
					{
						RemoveRoomPanel(entry);
					}
					else
					{
						m_roomPanelList[entry.Name].UpdatePanel(entry);
					}
				}
				else
				{
					if (!entry.RemovedFromList)
					{
						AddRoomPanel(entry);
					}
				}
			}

			m_cachedRoomList = roomList;
		}

		/// <summary>
		/// Creates a new Room Panel wich will show stats of a new Room.
		/// </summary>
		/// <param name="room"></param>
		private void AddRoomPanel(RoomInfo room)
		{
			var gbj = Instantiate(RoomPanel, Content, false);
			var panel = gbj.GetComponent<RoomPanel>();

			panel.SetRoom(room);
			panel.AddClickListener(() => OnJoinRoomClicked(room));
			m_roomPanelList.Add(room.Name, panel);
		}

		/// <summary>
		/// Deletes closed or not used Room.
		/// </summary>
		/// <param name="room"></param>
		private void RemoveRoomPanel(RoomInfo room)
		{
			var panel = m_roomPanelList[room.Name];
			m_roomPanelList.Remove(room.Name);
			Destroy(panel.gameObject);
		}

		/// <summary>
		/// To join a specific Room
		/// </summary>
		/// <param name="roomInfo">Contains information like name.</param>
		public void OnJoinRoomClicked(RoomInfo roomInfo)
		{
			m_doubleInput.RecordInput();

			if (m_doubleInput.RecordedDoubleInput())
			{
				m_roomSelected = true;
				OnRoomSelected?.Invoke();
				PhotonNetwork.JoinRoom(roomInfo.Name);
			}
		}

		/// <summary>
		/// Photon Callback.
		/// Only if Room joining was successful, scene will be loaded.
		/// </summary>
		public override void OnJoinedRoom()
		{
			if (!m_roomSelected) return;

			LoadingScreen.LoadScene(WaitingRoom.SceneIndex);
		}
	}
}