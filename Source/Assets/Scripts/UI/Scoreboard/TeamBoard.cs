using System.Collections.Generic;
using System.Linq;
using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;
using UI.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace UI.Scoreboard
{
	public class TeamBoard : MonoBehaviour
	{
		[SerializeField] private Color HeaderFriendlyColor = Color.green;
		[SerializeField] private Color HeaderEnemyColor = Color.red;
		[SerializeField] private Color LocalPlayerColor = Color.gray;
		[SerializeField] private Image Header = null;
		[SerializeField] private PlayerStats PlayerStats = null;
		[SerializeField] private RectTransform TargetParent = null;
		[SerializeField] private ValueDisplaySet TeamScore = null;

		private Dictionary<Player, PlayerStats> m_stats = new Dictionary<Player, PlayerStats>();
		private bool m_teamScore = false;

		/// <summary>
		/// To create a Teamboard.
		/// </summary>
		/// <param name="team">What Team</param>
		/// <param name="ownTeam">Local player in this team</param>
		public void Initialize(Team team, bool ownTeam)
		{
			SetColor(ownTeam);
			CreateTeam(team, ownTeam);
		}

		/// <summary>
		/// Enable/Disable Team score display.
		/// </summary>
		/// <param name="enable"></param>
		public void ShowTeamScore(bool enable)
		{
			m_teamScore = enable;
			TeamScore.gameObject.SetActive(enable);
		}

		/// <summary>
		/// Green for own , Red for other Team.
		/// </summary>
		/// <param name="ownTeam"></param>
		private void SetColor(bool ownTeam)
		{
			Header.color = ownTeam == true ? HeaderFriendlyColor : HeaderEnemyColor;
			TeamScore.SetImageColor(Header.color);
		}

		/// <summary>
		/// Create a slot for each Player.
		/// Local PLayer slot will be highlighted.
		/// </summary>
		/// <param name="team"></param>
		/// <param name="ownTeam"></param>
		private void CreateTeam(Team team, bool ownTeam)
		{
			var size = PhotonNetwork.PlayerList.ToList().Where(x => x.GetTeam() == team);

			foreach (var player in size)
			{
				var newStats = Instantiate(PlayerStats, TargetParent, false);
				m_stats.Add(player, newStats);
				if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					newStats.SetBackgroundColor(LocalPlayerColor);
				}

				newStats.SetName(player.NickName);
			}

			Sort();
			UpdateLayout();
		}

		/// <summary>
		/// Update player stats.
		/// </summary>
		/// <param name="targetPlayer">Player that will be updated.</param>
		/// <param name="props">ignored</param>
		public void Refresh(Player targetPlayer, Hashtable props)
		{
			if (m_stats.Count == 0) return;

			if (!m_stats.TryGetValue(targetPlayer, out PlayerStats)) return;

			var k = targetPlayer.GetKills();
			var d = targetPlayer.GetDeaths();
			var a = targetPlayer.GetAssists();
			var score = targetPlayer.GetScore();

			PlayerStats.SetKDA(k, d, a);
			PlayerStats.SetScore(score);

			Sort();

			if (m_teamScore)
			{
				UpdateTeamScore();
			}
		}

		/// <summary>
		/// Delete Stats from a specific Player.
		/// </summary>
		/// <param name="targetPlayer">Player who e.g left the match</param>
		public void DeletePlayerStats(Player targetPlayer)
		{
			if (!m_stats.TryGetValue(targetPlayer, out PlayerStats)) return;

			m_stats.Remove(targetPlayer);
			Destroy(PlayerStats);
			UpdateTeamScore();
			Sort();
			UpdateLayout();
		}

		/// <summary>
		/// Sums of all player scores.
		/// </summary>
		private void UpdateTeamScore()
		{
			var score = 0;
			foreach (var pair in m_stats)
			{
				score += pair.Key.GetScore();
			}

			TeamScore.SetText(score.ToString());
		}

		/// <summary>
		/// Sort Player stats after score. 
		/// </summary>
		private void Sort()
		{
			var stats = m_stats.ToArray();
			var sorted = stats.OrderByDescending(t => t.Key.GetScore()).ToList();

			for (var i = 0; i < sorted.Count; i++)
			{
				var targetStats = m_stats[sorted[i].Key];
				targetStats.SetRank(i + 1);

				targetStats.transform.SetSiblingIndex(i);
			}
		}

		private void UpdateLayout()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
			LayoutRebuilder.ForceRebuildLayoutImmediate(TargetParent);
		}
	}
}