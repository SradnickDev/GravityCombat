using System.Collections.Generic;
using Network.Extensions;
using Network.Match;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace UI.Scoreboard
{
	/// <summary>
	/// Create, sort and updates team boards
	/// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	public class Scoreboard : MonoBehaviour, IInRoomCallbacks
	{
		[SerializeField] private MatchModel MatchModel = null;
		[SerializeField] private TeamBoard TeamBoard = null;
		[SerializeField] private RectTransform BoardParent = null;
		private CanvasGroup m_canvasGroup = null;

		private Dictionary<Team, TeamBoard>
			m_boards = new Dictionary<Team, TeamBoard>(); //storing team with associated board

		private void Awake()
		{
			//Connect to Events and Callbacks
			m_canvasGroup = GetComponent<CanvasGroup>();
			MatchModel.OnMatchStart += Initialize;
			MatchModel.OnMatchEnded += ForceDisplay;
			PhotonNetwork.AddCallbackTarget(this);
		}

		public void OnDisable()
		{
			//disconnect from Events and Callbacks

			MatchModel.OnMatchStart -= Initialize;
			MatchModel.OnMatchEnded -= ForceDisplay;
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		private void Update()
		{
			ActivateBoard();
		}

		/// <summary>
		/// Player input, to activate / deactivate the Scoreboard
		/// </summary>
		private void ActivateBoard()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				if (m_canvasGroup.alpha >= 0.0f)
				{
					m_canvasGroup.alpha = 1;
				}
			}

			if (Input.GetKeyUp(KeyCode.Tab))
			{
				if (m_canvasGroup.alpha <= 1.0f)
				{
					m_canvasGroup.alpha = 0;
				}
			}
		}

		/// <summary>Used to react to an Event.</summary>
		private void Initialize()
		{
			CreateBoards();
		}

		/// <summary>
		/// Creates for each Team a board, enables team score display if more than one board is available.
		/// </summary>
		private void CreateBoards()
		{
			var players = PhotonNetwork.PlayerList;

			foreach (var t in players)
			{
				var team = t.GetTeam();
				if (m_boards.ContainsKey(team)) continue;

				var newBoard = Instantiate(TeamBoard, BoardParent, false);
				m_boards.Add(team, newBoard);

				var isLocalTeam = PhotonNetwork.LocalPlayer.GetTeam() == team;
				newBoard.Initialize(team, isLocalTeam);
			}

			//Activate Team Score View if more than one team is available 
			foreach (var pair in m_boards)
			{
				pair.Value.ShowTeamScore(m_boards.Count > 1);
			}

			Sort();
			UpdateLayout();
			m_canvasGroup.alpha = 0;
		}

		/// <summary>
		/// Force the Layout to Update.
		/// </summary>
		private void UpdateLayout()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
			LayoutRebuilder.ForceRebuildLayoutImmediate(BoardParent);
		}

		/// <summary>
		/// Local Board should always be on top.
		/// </summary>
		private void Sort()
		{
			foreach (var board in m_boards)
			{
				var isLocalTeam = PhotonNetwork.LocalPlayer.GetTeam() == board.Key;
				if (isLocalTeam)
				{
					board.Value.transform.SetAsFirstSibling();
				}
			}
		}

		/// <summary>
		/// Photon callback, to update each team scoreboard.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="properties"></param>
		public void OnPlayerPropertiesUpdate(Player player, Hashtable properties)
		{
			if (m_boards.Count == 0) return;

			foreach (var keyValuePair in m_boards)
			{
				keyValuePair.Value.Refresh(player, properties);
			}
		}

		/// <summary>
		/// Disables input and activates the Scoreboard.
		/// </summary>
		private void ForceDisplay()
		{
			m_canvasGroup.alpha = 1;
		}

		#region Photon Callback

		public void OnMasterClientSwitched(Player newMasterClient) { }

		public void OnPlayerEnteredRoom(Player newPlayer) { }

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			if (m_boards.Count == 0) return;

			foreach (var keyValuePair in m_boards)
			{
				keyValuePair.Value.DeletePlayerStats(otherPlayer);
			}
		}

		#endregion
	}
}