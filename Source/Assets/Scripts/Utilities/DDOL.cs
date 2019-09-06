using UnityEngine;

namespace Utilities
{
	public class DDOL : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(this);
		}
	}
}