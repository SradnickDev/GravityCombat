using Photon.Pun;
using UI.Flex;
using UnityEditor;
using UnityEngine;

namespace UI
{
	public class MainMenu : FlexControl
	{
		protected override void Start()
		{
			base.Start();
			ResetCursor();
		}

		private static void ResetCursor()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		public void Exit()
		{
			PhotonNetwork.Disconnect();
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}
#endif
			Application.Quit();
		}
	}
}