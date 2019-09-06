using PlayerBehaviour.General;
using PlayerBehaviour.Interface;
using UnityEngine;

namespace Utilities
{
	public class DummyModel : MonoBehaviour, IDamageable
	{
		[SerializeField] private MaterialFlicker Flicker = null;

		public bool ApplyDamage(float damage)
		{
			if (Flicker != null)
			{
				Flicker.Play();
			}

			return true;
		}
	}
}