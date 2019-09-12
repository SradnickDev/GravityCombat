using System.Collections.Generic;
using System.Text.RegularExpressions;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;


namespace SceneHandling
{
	[CreateAssetMenu(menuName = ("MapDatabase"), fileName = ("MapDatabase"))]
	public class SceneContainerDatabase : ScriptableObject
	{
		[ReorderableList] public List<SceneContainer> Maps = new List<SceneContainer>();

		public int Count
		{
			get { return Maps.Count; }
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			SetSceneIndex();
		}

		[Button]
		private void SetSceneIndex()
		{
			var scenes = EditorBuildSettings.scenes;
			var regex = new Regex(@"([^/]*/)*([\w\d\-]*)\.unity");

			for (var i = 0; i < scenes.Length; i++)
			{
				if (scenes[i] == null) continue;

				var sceneName = regex.Replace(scenes[i].path, "$2");
				var t = GetContainerWithSceneName(sceneName);
				t.SceneIndex = i;
			}
		}
#endif

		private SceneContainer GetContainerWithSceneName(string sceneName)
		{
			var sceneContainer = Maps.Find(x => x.Scene.name == sceneName);
			if (sceneContainer == null)
			{
				Debug.LogWarning(sceneName + " Map not found.");
				return null;
			}

			return sceneContainer;
		}

		public SceneContainer GetContainerWithMapName(string mapName)
		{
			var sceneContainer = Maps.Find(x => x.MapName == mapName);
			if (sceneContainer == null)
			{
				Debug.LogWarning(mapName + " Map not found.");
				return null;
			}

			return sceneContainer;
		}
	}
}