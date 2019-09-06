using System;
using System.Collections.Generic;
using Network.Extensions;
using Network.Gamemode;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI {
	public class GameHints : MonoBehaviour
	{
		[Serializable]
		public class Hint
		{
			public string Mode = string.Empty;
			public string[] Text;

			public Hint(string mode)
			{
				Mode = mode;
			}
		}

		[SerializeField] private Text HintDisplay = null;

		[SerializeField] private List<Hint> Hints = new List<Hint>()
		{
			new Hint("none")
		};


#if UNITY_EDITOR
		private void OnValidate()
		{
			var modes = GameModeBase.GetAll();

			foreach (var mode in modes)
			{
				var modeExists = Hints.FindIndex(x => x.Mode == mode) != -1;
				if (!modeExists)
				{
					Hints.Add(new Hint(mode));
				}
			}
		}
#endif

		private void Start()
		{
			var text = GetHint();
			SetHint(text);
		}

		private string GetHint()
		{
			var text = Hints[0].Text[Random.Range(0, Hints[0].Text.Length)];

			if (PhotonNetwork.InRoom)
			{
				var targetMode = PhotonNetwork.CurrentRoom.GetGameMode();
				var name = targetMode.GetType().Name;
				var hint = Hints.Find(x => x.Mode == name);
				text = hint.Text[Random.Range(0, hint.Text.Length)];
			}

			return text;
		}

		private void SetHint(string text)
		{
			HintDisplay.text = text;
		}
	}
}