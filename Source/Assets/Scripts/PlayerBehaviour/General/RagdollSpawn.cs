using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
#endif

namespace PlayerBehaviour.General
{
	/// <summary>
	/// Creates a Ragdoll if the GameObject which this script is attached to gets destroyed.
	/// Using Callbacks to determine the scene is not changing or the Application gets quieted.
	/// </summary>
	public class RagdollSpawn : MonoBehaviour
	{
		[SerializeField] private Ragdoll Ragdoll = null;
		[SerializeField] private SkinnedMeshRenderer Renderer = null;
		private readonly int m_albedo = Shader.PropertyToID("_Albedo");
		private bool m_isQuitting = false;
		private bool m_isSceneLoading = false;

		private void OnEnable()
		{
			SceneManager.activeSceneChanged += ActiveSceneChanged;
		}

		private void OnDisable()
		{
			SceneManager.activeSceneChanged -= ActiveSceneChanged;
		}

		private void ActiveSceneChanged(Scene arg0, Scene arg1)
		{
			m_isSceneLoading = true;
		}

		private void OnApplicationQuit()
		{
			m_isQuitting = true;
		}

		private void OnDestroy()
		{
			if (!m_isQuitting || !m_isSceneLoading)
			{
				var ragdollInstance = Instantiate(Ragdoll, transform.position, transform.rotation);
				ragdollInstance.Setup(Renderer.material.GetTexture(m_albedo));
			}
		}
	}
}