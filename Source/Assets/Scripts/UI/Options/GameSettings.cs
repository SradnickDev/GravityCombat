using Photon.Pun;
using UI.Login;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Options
{
	public class GameSettings : MonoBehaviour
	{
		public const string ScreenShakePref = "screenShakePref";
		public const string ChatPref = "chatPref";
		public const string PingPref = "pingPref";
		public const string MovementPref = "movementPref";

		[SerializeField] private Toggle ScreenShakeToggle = null;
		[SerializeField] private Toggle ChatToggle = null;
		[SerializeField] private Toggle DisplayPingToggle = null;
		[SerializeField] private Toggle MovementToggle = null;
		[SerializeField] private SceneContainer.SceneContainer MenuContainer = null;

		private void Start()
		{
			InitGameSettings();
		}

		private void OnEnable()
		{
			InitGameSettings();
		}

		#region GameSettings

		private void InitGameSettings()
		{
			ScreenShakeToggle.isOn = Options.GetBool(ScreenShakePref, true);
			ChatToggle.isOn = Options.GetBool(ChatPref, true);
			DisplayPingToggle.isOn = Options.GetBool(PingPref, false);
			MovementToggle.isOn = Options.GetBool(MovementPref, false);

			ScreenShakeToggle.onValueChanged.AddListener(OnScreenShakeToggleChanged);
			ChatToggle.onValueChanged.AddListener(OnChatToggleChanged);
			DisplayPingToggle.onValueChanged.AddListener(OnPingToggleChanged);
			MovementToggle.onValueChanged.AddListener(OnMovementToggleChanged);
		}

		private void OnScreenShakeToggleChanged(bool value)
		{
			Options.SetBool(ScreenShakePref, value);
		}

		private void OnChatToggleChanged(bool value)
		{
			Options.SetBool(ChatPref, value);
		}

		private void OnPingToggleChanged(bool value)
		{
			Options.SetBool(PingPref, value);
		}

		private void OnMovementToggleChanged(bool value)
		{
			Options.SetBool(MovementPref, value);
		}

		public void ResetName()
		{
			PlayerPrefs.DeleteKey(LoginModel.NicknamePref);
			PhotonNetwork.Disconnect();
			LoadingScreen.LoadScene(MenuContainer.SceneIndex);
		}

		#endregion
	}
}