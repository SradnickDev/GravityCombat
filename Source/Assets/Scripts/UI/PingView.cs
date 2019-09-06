using Photon.Pun;
using UI.Options;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class PingView : MonoBehaviour
	{
		[SerializeField] private Text Label = null;

		private void Update()
		{
			if (Options.Options.GetBool(GameSettings.PingPref, false))
			{
				Label.text = PhotonNetwork.GetPing().ToString();
			}
			else
			{
				Label.text = string.Empty;
			}
		}
	}
}