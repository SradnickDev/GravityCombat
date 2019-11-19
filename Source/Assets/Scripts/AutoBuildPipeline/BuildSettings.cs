#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SradnickDev.BuildPipe
{
	public class BuildSettings : EditorWindow
	{
		private bool m_increaseVersion = false;
		private bool m_useSuffix = false;
		private BuildOptions m_buildOptions = BuildOptions.None;

		[MenuItem("Build Pipeline / Build %#l")]
		private static void Init()
		{
			// Get existing open window or if none, make a new one:
			var window = GetWindow(typeof(BuildSettings));
			window.titleContent = new GUIContent("Build Pipeline Settings");
			window.position = new Rect(new Vector2(810, 440), new Vector2(300, 180));
			window.Show();
		}

		/// <summary>
		/// Small Window with build settings.
		/// </summary>
		private void OnGUI()
		{
			GUILayout.Label("Build Pipeline Settings", EditorStyles.boldLabel);
			GUILayout.Label("Current Build : " + PlayerSettings.macOS.buildNumber, EditorStyles.label);
			var currentVersion = PlayerSettings.bundleVersion;
			GUILayout.Label("Current Version : " + currentVersion, EditorStyles.label);
			m_increaseVersion = EditorGUILayout.Toggle("Increase Version : ", m_increaseVersion);
			m_useSuffix = EditorGUILayout.Toggle("Suffix : ", m_useSuffix);
			m_buildOptions = (BuildOptions) EditorGUILayout.EnumPopup("BuildOptions : ", m_buildOptions);
			GUILayout.Space(30);

			if (GUILayout.Button("Build"))
			{
				AutoVersion.HandleVersion(m_increaseVersion, m_useSuffix);
				Build();
			}
		}

		/// <summary>
		/// Start Build Process.
		/// </summary>
		private void Build()
		{
			string[] scenePaths = new string[EditorBuildSettings.scenes.Length];

			for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				var currentScene = EditorBuildSettings.scenes[i].path;
				scenePaths[i] = currentScene;
			}

			var path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
			var projectName = "/" + PlayerSettings.productName + ".exe";
			BuildPipeline.BuildPlayer(scenePaths, path + projectName, BuildTarget.StandaloneWindows, m_buildOptions);
		}
	}
}
#endif