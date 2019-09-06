using UnityEngine;

namespace PlayerBehaviour.Weapon
{
	/// <summary>
	/// Acts as a Trigger, if a Weapon stuck in ground, Player cant shoot.
	/// </summary>
	public class WeaponCollision : MonoBehaviour
	{
		private const int GroundLayer = 10;
		private bool m_colliding = false;

		private void OnTriggerStay(Collider other)
		{
			if (other.gameObject.layer != GroundLayer) return;

			m_colliding = true;
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.layer != GroundLayer) return;

			m_colliding = false;
		}

		public bool IsColliding()
		{
			return m_colliding;
		}
	}
}