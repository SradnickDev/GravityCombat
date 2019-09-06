using UnityEngine;

namespace UI
{
	public class TimerWarning : MonoBehaviour
	{
		[SerializeField] private AudioSource AudioSource = null;
		[SerializeField] private Animator Animator = null;
		private int Play = Animator.StringToHash("Play");
		private bool m_started = false;

		public void Start()
		{
			if (m_started == false)
			{
				Animator.SetBool(Play, true);
				PlaySound();
				m_started = true;
			}
		}

		public void Stop()
		{
			if (m_started == true)
			{
				Animator.SetBool(Play, false);
				AudioSource.Stop();
				m_started = false;
			}
		}

		private void PlaySound()
		{
			if (AudioSource != null)
			{
				AudioSource.Play();
			}
		}
	}
}