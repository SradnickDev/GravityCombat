using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Note:
// From an older Project, not entirely written for Gravity Combat.
// Received some major changes because Photon Puns Api from Pun 1 to Pun 2 changed.

namespace UI
{
	public class LoadingScreen : MonoBehaviour
	{
		[SerializeField] private Image FadeOverlay = null;
		[SerializeField] private float FadeDuration = 0.25f;

		private float m_loadingOp;
		private static int m_sceneToLoad = -1;
		private static int m_loadingSceneIndex = 1; //Loading Scene, Build Settings Index

		/// <summary>
		/// Opens Loading Screen scene.
		/// </summary>
		/// <param name="sceneIndex">Will be stored to load.</param>
		public static void LoadScene(int sceneIndex)
		{
			m_sceneToLoad = sceneIndex;
			PhotonNetwork.IsMessageQueueRunning = false;
			SceneManager.LoadScene(m_loadingSceneIndex);
		}

		private void Start()
		{
			StartCoroutine(StartLoadingProgress(m_sceneToLoad));
		}

		private IEnumerator StartLoadingProgress(int sceneIndex)
		{
			OnStartLoading();
			FadeIn();
			yield return new WaitForSeconds(FadeDuration);

			StartAsyncOp(sceneIndex);

			while (DoneLoading() == false)
			{
				yield return null;
			}

			FadeOut();
			yield return new WaitForSeconds(FadeDuration);
		}

		private void StartAsyncOp(int sceneIndex)
		{
			PhotonNetwork.LoadLevel(sceneIndex);
			m_loadingOp = PhotonNetwork.LevelLoadingProgress;
		}

		private bool DoneLoading()
		{
			return m_loadingOp >= 0.9f;
		}

		private void FadeIn()
		{
			FadeOverlay.CrossFadeAlpha(0, FadeDuration, true);
		}

		private void FadeOut()
		{
			FadeOverlay.CrossFadeAlpha(1, FadeDuration, true);
		}

		private void OnStartLoading()
		{
			FadeOverlay.gameObject.SetActive(true);
		}
	}
}