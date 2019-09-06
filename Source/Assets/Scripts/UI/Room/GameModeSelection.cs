using System;
using System.Linq;
using Network.Gamemode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Room
{
	public class GameModeSelection : MonoBehaviour
	{
		public Action<string> OnGameModeSelected;

		[SerializeField] private Dropdown ModeDropDown = null;

		private void Start()
		{
			ModeDropDown.ClearOptions();
			SetGameMode();
			if (ModeDropDown != null)
			{
				ModeDropDown.onValueChanged.AddListener(delegate { DropdownValueChanged(ModeDropDown); });
			}

			DropdownValueChanged(ModeDropDown);
		}

		/// <summary>
		/// Creates Dropdown Options as string.
		/// </summary>
		private void SetGameMode()
		{
			var modes = GameModeBase.GetAll();

			for (var i = 0; i < modes.Count; i++)
			{
				modes[i] = string.Concat(modes[i].Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
			}

			ModeDropDown.AddOptions(modes);
		}

		/// <summary>
		/// Receive GameModeDropdown changes.
		/// </summary>
		/// <param name="change"></param>
		private void DropdownValueChanged(Dropdown change)
		{
			var pickedMode = change.options[change.value].text;
			GameModeSelected(pickedMode);
		}

		/// <summary>
		/// Get GameMode from selected Dropdown field and fires Event.
		/// </summary>
		private void GameModeSelected(string mode)
		{
			OnGameModeSelected?.Invoke(mode);
		}
	}
}