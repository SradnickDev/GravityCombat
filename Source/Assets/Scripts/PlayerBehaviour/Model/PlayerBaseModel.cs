using Photon.Pun;
using UnityEngine;

namespace PlayerBehaviour.Model
{
	/// <summary>
	/// Bridge between diff Models.
	/// </summary>
	[RequireComponent(typeof(PlayerMovementModel))]
	[RequireComponent(typeof(PlayerBootsModel))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PhotonView))]
	public class PlayerBaseModel : MonoBehaviour
	{
		public PhotonView PhotonView { get; private set; }
		protected PlayerMovementModel MovementModel = null;
		protected PlayerBootsModel Boots = null;
		protected Rigidbody Rigidbody = null;

		public virtual void Start()
		{
			PhotonView = GetComponent<PhotonView>();
			Rigidbody = GetComponent<Rigidbody>();
			MovementModel = GetComponent<PlayerMovementModel>();
			Boots = GetComponent<PlayerBootsModel>();
		}
	}
}