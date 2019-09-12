using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SceneHandling
{
	[CustomEditor(typeof(SceneContainer))]
	public class SceneContainerInspector : Editor
	{
		private SceneContainer m_sceneContainer = null;
		private SerializedObject m_target = null;

		private void OnEnable()
		{
			m_sceneContainer = (SceneContainer) target;
			m_target = new SerializedObject(m_sceneContainer);
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if (GUILayout.Button("Load Scene"))
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(m_sceneContainer.Scene));
			}
		}
	}
}
#endif