using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Options
{
	public class AudioSettings : MonoBehaviour
	{
		public const string MasterPref = "masterPref";
		public const string MusicPref = "musicPref";
		public const string VfxPref = "vfxPref";

		[SerializeField] private AudioMixer AudioMixer = null;
		[SerializeField] private Slider MasterSlider = null;
		[SerializeField] private Slider MusicSlider = null;
		[SerializeField] private Slider VfxSlider = null;

		private void Start()
		{
			InitSoundSettings();
		}

		private void OnEnable()
		{
			InitSoundSettings();
		}

		#region AudioSettings

		public void InitSoundSettings()
		{
			MasterSlider.value = PlayerPrefs.GetFloat(MasterPref, 0.75f);
			VfxSlider.value = PlayerPrefs.GetFloat(VfxPref, 0.75f);
			MusicSlider.value = PlayerPrefs.GetFloat(MusicPref, 0.75f);

			OnMasterSliderChanged(MasterSlider.value);
			OnMusicSliderChanged(VfxSlider.value);
			OnVfxSliderChanged(MusicSlider.value);

			MasterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
			MusicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
			VfxSlider.onValueChanged.AddListener(OnVfxSliderChanged);
		}

		private void OnMasterSliderChanged(float value)
		{
			AudioMixer.SetFloat("Master", 20f * Mathf.Log10(value));
			PlayerPrefs.SetFloat(MasterPref, value);
		}

		private void OnMusicSliderChanged(float value)
		{
			AudioMixer.SetFloat("Music", 20f * Mathf.Log10(value));
			PlayerPrefs.SetFloat(MusicPref, value);
		}

		private void OnVfxSliderChanged(float value)
		{
			AudioMixer.SetFloat("VFX", 20f * Mathf.Log10(value));
			PlayerPrefs.SetFloat(VfxPref, value);
		}

		#endregion
	}
}