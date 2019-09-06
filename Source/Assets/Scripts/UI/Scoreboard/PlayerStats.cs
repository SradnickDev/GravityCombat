using UnityEngine;
using UnityEngine.UI;

namespace UI.Scoreboard
{
	/// <summary>
	/// Displays Player Stats for Scoreboard.
	/// </summary>
	public class PlayerStats : MonoBehaviour
	{
		[SerializeField] private Image Background = null;
		[SerializeField] private Text Rank = null;
		[SerializeField] private Text Name = null;
		[SerializeField] private Text KDA = null;
		[SerializeField] private Text Score = null;

		public void SetBackgroundColor(Color color)
		{
			Background.color = color;
		}

		public void SetRank(int idx)
		{
			Rank.text = idx.ToString();
		}

		public void SetName(string nickName)
		{
			Name.text = nickName;
		}

		public void SetKDA(int kills, int deaths, int assists)
		{
			KDA.text = $"{kills} / {deaths} / {assists}";
		}

		public void SetScore(float value)
		{
			Score.text = value.ToString();
		}
	}
}