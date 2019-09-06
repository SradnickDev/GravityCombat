using System.Collections;
using UnityEngine;

namespace PlayerBehaviour.General
{
	public class Shield : MonoBehaviour
	{
		[SerializeField] private float Duration = 5;
		[SerializeField] private ParticleSystem ShieldEffect = null;

		private bool m_isActive = true;

		public bool IsActive
		{
			get { return m_isActive; }
		}

		private void Start()
		{
			ActivateShield();
		}

		private void ActivateShield()
		{
			StartCoroutine(SpawnInvincible());
		}

		private IEnumerator SpawnInvincible()
		{
			ShieldEffect.Play(true);
			m_isActive = true;
			yield return new WaitForSeconds(Duration);
			DisableShield();
		}

		public void DisableShield()
		{
			ShieldEffect.Stop(true);
			m_isActive = false;
		}
	}
}