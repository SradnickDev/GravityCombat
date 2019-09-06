using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Options
{
	public class GraphicSettings : MonoBehaviour
	{
		[SerializeField] private Dropdown ResolutionDropDown = null;
		[SerializeField] private Toggle FullscreenToggle = null;
		[SerializeField] private Toggle AntiAliasingToggle = null;
		[SerializeField] private Toggle VSyncToggle = null;

		private void Start()
		{
			SetResolutionDropDownOptions();
			InitGraphicSettings();
		}

		private void OnDestroy()
		{
			FullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChange);

			AntiAliasingToggle.onValueChanged.RemoveListener(OnAntiAliasingToggleChanged);
			VSyncToggle.onValueChanged.RemoveListener(OnVSyncToggleChanged);
		}

		#region GraphicSettings

		private void InitGraphicSettings()
		{
			FullscreenToggle.isOn = Screen.fullScreen;

			AntiAliasingToggle.isOn = QualitySettings.antiAliasing >= 1;
			VSyncToggle.isOn = QualitySettings.vSyncCount >= 1;

			ResolutionDropDown.onValueChanged.AddListener(OnResolutionDropDownChanged);
			FullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChange);

			AntiAliasingToggle.onValueChanged.AddListener(OnAntiAliasingToggleChanged);
			VSyncToggle.onValueChanged.AddListener(OnVSyncToggleChanged);
		}

		private void SetResolutionDropDownOptions()
		{
			ResolutionDropDown.ClearOptions();
			var resolution = Screen.resolutions;
			var matchingResolution = 0;
			var options = new List<string>();

			for (var i = 0; i < resolution.Length; i++)
			{
				var resInfo = resolution[i];
				var res = $"{resInfo.width} x {resInfo.height}  {resInfo.refreshRate} Mhz";

				if (!options.Contains(res))
				{
					options.Add(res);
				}

				if (resInfo.height == Screen.height &&
					resInfo.width == Screen.width)
				{
					matchingResolution = i;
				}
			}

			ResolutionDropDown.AddOptions(options);
			ResolutionDropDown.value = matchingResolution;
			ResolutionDropDown.onValueChanged.RemoveListener(OnResolutionDropDownChanged);
		}

		private void OnResolutionDropDownChanged(int index)
		{
			Debug.Log("asd");
			var resolution = Screen.resolutions;
			var targetResolution = resolution[index];

			Screen.SetResolution(targetResolution.width, targetResolution.height, Screen.fullScreenMode,
								targetResolution.refreshRate);
		}

		private void OnFullscreenToggleChange(bool value)
		{
			Screen.fullScreen = value;
		}

		private void OnAntiAliasingToggleChanged(bool value)
		{
			QualitySettings.antiAliasing = value ? 1 : 0;
		}

		private void OnVSyncToggleChanged(bool value)
		{
			QualitySettings.vSyncCount = value ? 1 : 0;
		}

		#endregion
	}
}