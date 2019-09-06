using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Network.Gamemode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Match
{
	/// <summary>
	/// Used to specify and query a Game Mode. 
	/// </summary>
	public class MatchmakingButton : MonoBehaviour
	{
		[SerializeField] private Matchmaking Matchmaking = null;
		[SerializeField] private Text DisplayText = null;
		[SerializeField] private Button Button = null;
		[SerializeField, ReadOnly] private List<string> GameModes = new List<string>();
		[SerializeField] private int GameModeIndex = 0;

		[Button]
		private void LoadGameModes()
		{
			GameModes = GameModeBase.GetAll();
		}

		private void Start()
		{
			DisplayText.text = string
				.Concat(GameModes[GameModeIndex].Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
			Button.onClick.AddListener(() => Matchmaking.OnPickMode(GameModes[GameModeIndex]));
		}
	}
}