using Network.Extensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerBehaviour.General
{
	public class NamePlate : MonoBehaviour
	{
		[SerializeField] private Text NameDisplay = null;
		[SerializeField] private PhotonView PhotonView = null;
		[SerializeField] private Color Friendly = Color.yellow;
		[SerializeField] private Color Enemy = Color.red;

		private void Start()
		{
			if (PhotonView != null)
			{
				if (PhotonView.IsMine)
				{
					NameDisplay.enabled = false;
					return;
				}
			}

			var friendly = PhotonView.Owner.IsFriendly();
			NameDisplay.color = friendly ? Friendly : Enemy;
			NameDisplay.text = PhotonView.Owner.NickName;
		}
	}
}