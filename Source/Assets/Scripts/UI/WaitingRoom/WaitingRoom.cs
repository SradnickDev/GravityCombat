using SCT;
using Photon.Pun;
using UnityEngine;
using UI.Utilities;
using SceneHandling;
using Network.Match;
using Photon.Realtime;
using System.Collections;
using Network.Extensions;
using Network.WaitingRoom;

namespace UI.WaitingRoom
{
	/// <summary>
	/// Handles Player in WaitingRoom and Timers.
	/// </summary>
	[RequireComponent(typeof(MatchSpawn))]
	[RequireComponent(typeof(MatchRespawn))]
	public class WaitingRoom : WaitingRoomPart
	{
		[SerializeField] private MatchSpawn MatchSpawn = null;
		[SerializeField] private SceneContainerDatabase SceneContainerDatabase = null;
		[SerializeField] private SceneContainer Lobby = null;
		[SerializeField] private ValueDisplaySet InfoLabel = null;
		[SerializeField] ScriptableTextDisplay ScriptableTextDisplay = null;
		[SerializeField] private string MasterClientInfoText = "Press F5 to Start";
		[SerializeField] private string ClientInfoText = "Wait for Host to Start";

		public override void Start()
		{
			base.Start();
			Cursor.lockState = CursorLockMode.None;

			PhotonNetwork.LocalPlayer.SetTeam(Team.Aggressive);
			if (PhotonNetwork.IsMasterClient)
			{
				StartCoroutine(SetPing());

				InfoLabel.SetText(MasterClientInfoText);
			}
			else
			{
				InfoLabel.SetText(ClientInfoText);
			}
		}
		
		public override void Update()
		{
			base.Update();

			if (Input.GetKeyDown(KeyCode.F5))
			{
				ForcedStart();
			}
		}

		#region Callbacks

		protected override void OnStateChanged(WaitingRoomState state)
		{
			base.OnStateChanged(state);

			if (state == WaitingRoomState.TimerIsOver)
			{
				if (InfoLabel)
				{
					InfoLabel.gameObject.SetActive(false);
				}

				if (ScriptableTextDisplay != null)
				{
					ScriptableTextDisplay.InitializeScriptableText(6, transform.position, "Get Ready");
				}
			}
		}

		public override void PlayerJoined(Player newPlayer)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				newPlayer.SetTeam(Team.Aggressive);
			}
		}

		public override void PlayerLeft(Player player)
		{
		}

		public override void PlayerIsReady(Player player)
		{
			MatchSpawn.SingleSpawn(player);
		}

		public override void MasterClientLeft()
		{
			PhotonNetwork.LeaveRoom();
			LoadingScreen.LoadScene(Lobby.SceneIndex);
		}

		public override void MatchStart()
		{
			LoadPickedMap();
		}

		#endregion

		private void LoadPickedMap()
		{
			PhotonNetwork.LocalPlayer.ResetProperties();
			PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
			
			var mapName = PhotonNetwork.CurrentRoom.GetMap();
			var map = SceneContainerDatabase.GetContainerWithMapName(mapName);
			LoadingScreen.LoadScene(map.SceneIndex);
		}

		private IEnumerator SetPing()
		{
			PhotonNetwork.CurrentRoom.SetPing(PhotonNetwork.GetPing());
			yield return new WaitForSeconds(2);
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsOpen)
			{
				StartCoroutine(SetPing());
			}
		}
	}
}