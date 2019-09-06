using UnityEngine;

namespace Music
{
	[RequireComponent(typeof(AudioSource))]
	public class BackgroundMusic : MonoBehaviour
	{
		public int Priority = 0;
		private AudioSource m_audioSource = null;

		private void OnEnable()
		{
			DestroyDuplication();

			m_audioSource = GetComponent<AudioSource>();
			DontDestroyOnLoad(this.gameObject);

			if (!m_audioSource.isPlaying)
			{
				m_audioSource.loop = true;
				m_audioSource.Play();
			}
		}

		/// <summary>
		/// Destroy Background Music Object with a low priority.
		/// </summary>
		private void DestroyDuplication()
		{
			BackgroundMusic[] backgroundTracks = FindObjectsOfType<BackgroundMusic>();

			if (backgroundTracks.Length == 0) return;

			foreach (var music in backgroundTracks)
			{
				if (music == this) continue;

				if (music.Priority > Priority)
				{
					Destroy(this.gameObject);
				}

				if (music.Priority < Priority)
				{
					Destroy(music.gameObject);
				}

				if (music.Priority == Priority)
				{
					Destroy(music.gameObject);
				}
			}
		}

		/// <summary>
		/// Change clip on the fly.
		/// </summary>
		/// <param name="newClip">New Clip to play</param>
		public void ChangeClip(AudioClip newClip)
		{
			if (m_audioSource.clip == newClip) return;

			m_audioSource.Stop();
			m_audioSource.clip = newClip;
			m_audioSource.Play();
		}
	}
}