using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
	public class TopPanel : MonoBehaviour
	{
		public UnityEvent OnOpen;
		public UnityEvent OnClose;

		[SerializeField] SceneContainer.SceneContainer LobbySceneContainer = null;
		private bool m_isActive = false;

		void Start()
		{
			Cursor.lockState = CursorLockMode.Confined;
			UiSelection.Instance.OnSelectionChanged += OnSelectionChanged;
		}

		private void OnSelectionChanged(GameObject newSelection)
		{
			if (newSelection != gameObject)
			{
				OnClose?.Invoke();
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				m_isActive = !m_isActive;
				if (m_isActive)
				{
					Open();
				}
				else
				{
					Close();
				}
			}
		}

		public void Open()
		{
			OnOpen?.Invoke();
			UiSelection.Instance.AddSelection(gameObject);
			m_isActive = true;
		}

		public void Close()
		{
			OnClose?.Invoke();
			UiSelection.Instance.RemoveSelection(gameObject);
			m_isActive = false;
		}

		public void LeaveCurrentRoom()
		{
			PhotonNetwork.LeaveRoom();
			LoadingScreen.LoadScene(LobbySceneContainer.SceneIndex);
		}

		private void OnEnable()
		{
			UiSelection.Instance.OnSelectionChanged -= OnSelectionChanged;
		}
	}
}