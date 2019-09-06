using System.Collections;
using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace UI.KillFeed
{
	/// <summary>
	/// Display Kill Feed.
	/// </summary>
	public class KillFeedView : MonoBehaviour
	{
		private const string Yellow = "#ffff00ff";
		private const string Red = "#ff0000ff";
		private const string Green = "#00ff00ff";

		[SerializeField] private float VisibleTime = 10;
		[SerializeField] private Text Feed = null;

		private Coroutine m_hide = null;

		private void Start()
		{
			Feed.text = string.Empty;
		}

		/// <summary>
		/// Set infos for new Feed.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="killedPlayer"></param>
		public void SetFeed(Player player, Player killedPlayer)
		{
			Feed.CrossFadeAlpha(1, 0, false);

			var playerColor = player.IsFriendly() ? Yellow : Red;

			if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				playerColor = Green;
			}

			Feed.text = $"<b><color={playerColor}>{player.NickName}</color></b> killed <b>{killedPlayer.NickName}</b>";

			if (m_hide != null)
			{
				StopCoroutine(m_hide);
			}

			m_hide = StartCoroutine(Hide());
		}

		/// <summary>
		/// After short delay Feed stars fading out
		/// </summary>
		/// <returns></returns>
		private IEnumerator Hide()
		{
			yield return new WaitForSeconds(VisibleTime);
			Feed.CrossFadeAlpha(0, 0.25f, false);
		}
	}
}