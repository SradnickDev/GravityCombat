﻿using UnityEngine;

namespace Music
{
	public class MusicChangeTrigger : MonoBehaviour
	{
		[SerializeField] private AudioClip NewClip = null;

		void Start() => FindObjectOfType<BackgroundMusic>().ChangeClip(NewClip);
	}
}