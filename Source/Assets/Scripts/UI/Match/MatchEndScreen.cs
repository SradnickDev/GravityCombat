using System.Collections;
using Network.Extensions;
using Network.Gamemode;
using Network.Match;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Match
{
	[RequireComponent(typeof(MatchModel))]
	public class MatchEndScreen : MonoBehaviour
	{
		[SerializeField] private MatchModel MatchModel = null;
		[SerializeField] private Text EndDisplay = null;
		private GameModeBase m_modeBase = null;

		private void Start()
		{
			EndDisplay.enabled = false;
			MatchModel.OnMatchEnded += Show;
			m_modeBase = PhotonNetwork.CurrentRoom.GetGameMode();
		}

		/// <summary>
		/// Display Information about who eg. Won the Game.
		/// </summary>
		private void Show()
		{
			if (EndDisplay == null) return;
			StartCoroutine(DelayMsg());
		}

		private IEnumerator DelayMsg()
		{
			yield return new WaitForSeconds(2);
			EndDisplay.enabled = true;
			EndDisplay.text = m_modeBase.GetLeading();
		}

		private void OnDisable()
		{
			MatchModel.OnMatchEnded -= Show;
		}
	}
}