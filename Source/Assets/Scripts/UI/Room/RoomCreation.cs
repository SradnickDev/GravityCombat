using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;
using UI.Flex;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace UI.Room
{
	/// <summary>
	/// Handles creation of Room.
	/// </summary>
	[RequireComponent(typeof(MapSelection))]
	[RequireComponent(typeof(GameModeSelection))]
	[RequireComponent(typeof(RoomSettings))]
	public class RoomCreation : MonoBehaviourPunCallbacks, IFlexScreen
	{
		[Header("Create Room References")] [SerializeField]
		private InputField RoomNameField = null;

		[SerializeField] private Button CreateButton = null;

		[Header("Map Preview References")] [SerializeField]
		private SceneContainer.SceneContainer WaitingRoom = null;

		private SceneContainer.SceneContainer m_pickedSceneContainer = null;
		private string m_pickedGameMode = "";
		private MapSelection m_mapSelection = null;
		private GameModeSelection m_gameModeSelection = null;
		private RoomSettings m_roomSettings = null;

		public override void OnEnable()
		{
			base.OnEnable();

			m_mapSelection = GetComponent<MapSelection>();
			m_gameModeSelection = GetComponent<GameModeSelection>();
			m_roomSettings = GetComponent<RoomSettings>();

			m_mapSelection.OnMapSelected += SetMap;
			m_gameModeSelection.OnGameModeSelected += SetGameMode;
		}

		private void Start()
		{
			CreateButton.onClick.AddListener(CreateRoom);
		}

		/// <summary> IFlexScreen </summary>
		public void Open()
		{
			gameObject.SetActive(true);
		}

		/// <summary> IFlexScreen </summary>
		public void Close()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Called from Event when a Map is selected.
		/// </summary>
		/// <param name="sceneContainer"></param>
		private void SetMap(SceneContainer.SceneContainer sceneContainer)
		{
			m_pickedSceneContainer = sceneContainer;
		}

		/// <summary>
		/// Cache GameMode.
		/// </summary>
		private void SetGameMode(string mode)
		{
			mode = mode.Replace(" ", string.Empty);
			m_pickedGameMode = mode;
		}

		/// <summary>
		/// Room Creation
		/// </summary>
		private void CreateRoom()
		{
			var customRoomOption = CustomRoomOption();
			var roomOptions = RoomOptions(customRoomOption);

			var fixedRoomName = FixRoomName();
			var created = PhotonNetwork.CreateRoom(fixedRoomName, roomOptions);

			if (created) return;
			var rndRoomHash = RoomHash();

			fixedRoomName = RoomNameField.text + rndRoomHash;
			PhotonNetwork.CreateRoom(fixedRoomName, roomOptions, TypedLobby.Default);
		}

		#region Room Creation

		/// <summary>Configure Custom Room Options.</summary>
		private Hashtable CustomRoomOption()
		{
			var customRoomOption = new Hashtable()
			{
				{RoomProperties.Map, m_pickedSceneContainer.MapName},
				{RoomProperties.GameMode, m_pickedGameMode},
				{RoomProperties.Time, m_roomSettings.MatchTime},
				{RoomProperties.WeaponSpawnTime, m_roomSettings.WeaponSpawnTime},
				{RoomProperties.Round, m_roomSettings.Rounds},
			};
			return customRoomOption;
		}

		/// <summary>
		/// Configure RoomOptions
		/// </summary>
		/// <param name="customRoomOption"></param>
		/// <returns></returns>
		private RoomOptions RoomOptions(Hashtable customRoomOption)
		{
			var roomOptions = new RoomOptions()
			{
				BroadcastPropsChangeToAll = true,
				PlayerTtl = 0,    //remove the client immediately
				EmptyRoomTtl = 0, //remove the room immediately
				IsVisible = false,
				IsOpen = false,
				DeleteNullProperties = true,
				PublishUserId = true,
				MaxPlayers = m_roomSettings.PlayerCount,
				CustomRoomPropertiesForLobby = new[] //sets TKey, for visibility in lobby
					{RoomProperties.Map, RoomProperties.GameMode, RoomProperties.Ping},
				CustomRoomProperties = customRoomOption,
			};
			return roomOptions;
		}

		/// <summary>
		/// Replaces empty name with random name.
		/// </summary>
		/// <returns></returns>
		private string FixRoomName()
		{
			var roomName = RoomNameField.text;
			roomName = roomName.Replace(" ", string.Empty);

			var fixedRoomName = RoomNameField.text == "" ? RoomHash() : RoomNameField.text;
			return fixedRoomName;
		}

		/// <summary>Create random hash.</summary>
		private string RoomHash()
		{
			return "#" + Random.Range(0, 9999);
		}

		/// <summary>Photon API callback</summary>
		public override void OnJoinedRoom()
		{
			PhotonNetwork.LocalPlayer.SetReady(false);
			LoadingScreen.LoadScene(WaitingRoom.SceneIndex);
		}

		#endregion

		public override void OnDisable()
		{
			base.OnDisable();

			m_mapSelection.OnMapSelected -= SetMap;
			m_gameModeSelection.OnGameModeSelected -= SetGameMode;
		}
	}
}