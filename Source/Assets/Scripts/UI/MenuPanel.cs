using Photon.Pun;
using UI.Flex;
using UnityEditor;
using UnityEngine;

namespace UI
{
	public class MenuPanel : FlexScreen
	{
		public void Quit()
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