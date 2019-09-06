using System;
using NaughtyAttributes;
using UnityEngine;

namespace UI
{
	[CreateAssetMenu()]
	public class HitEvent : ScriptableObject
	{
		public Action OnHit;

		public void AddListener(Action method)
		{
			OnHit += method;
		}

		public void RemoveListener(Action method)
		{
			OnHit -= method;
		}

		[Button()]
		public void Invoke()
		{
			OnHit?.Invoke();
		}

		private void OnDisable()
		{
			OnHit = null;
		}
	}
}