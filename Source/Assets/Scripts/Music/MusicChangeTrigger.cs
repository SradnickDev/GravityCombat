using UnityEngine;

namespace Music
{
	public class MusicChangeTrigger : MonoBehaviour
	{
		[SerializeField] private AudioClip NewClip = null;

		void Start()
		{
			ChangeClip();
		}

		private void ChangeClip()
		{
			var backgroundMusic = FindObjectOfType<BackgroundMusic>();
			backgroundMusic.ChangeClip(NewClip);
		}
	}
}