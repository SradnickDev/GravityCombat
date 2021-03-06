﻿using UnityEngine;

namespace Music
{
	[RequireComponent(typeof(AudioSource))]
	public class BackgroundMusic : MonoBehaviour
	{
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
		
		private void DestroyDuplication()
		{
			var backgroundMusic = FindObjectsOfType<BackgroundMusic>();

			foreach (var music in backgroundMusic)
			{
				if (music != this) Destroy(music.gameObject);
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