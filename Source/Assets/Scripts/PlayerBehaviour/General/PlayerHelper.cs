using Network.Extensions;
using Photon.Pun;
using PlayerBehaviour.Interface;
using UnityEngine;

namespace PlayerBehaviour.General
{
	public static class PlayerHelper
	{
		/// <summary>
		/// True if target is not in same team or is a dummy.
		/// </summary>
		/// <param name="gbj"></param>
		/// <returns></returns>
		public static bool ValidateTarget(this GameObject gbj)
		{
			var targetView = gbj.transform.GetComponent<PhotonView>();
			var target = gbj.transform.GetComponent<IDamageable>();

			if (target != null && targetView == null || targetView != null && target != null &&
				!targetView.Owner.IsFriendly() && !targetView.IsMine)
			{
				return true;
			}

			return false;
		}
	}
}