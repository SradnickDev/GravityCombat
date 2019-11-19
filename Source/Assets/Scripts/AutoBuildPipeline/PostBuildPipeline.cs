#if UNITY_EDITOR
using System.Text;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SradnickDev.BuildPipe
{
	public class PostBuildPipeline : IPostprocessBuildWithReport
	{
		public int callbackOrder
		{
			get { return 0; }
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			var appName = PlayerSettings.productName;
			var version = "v" + PlayerSettings.bundleVersion;
			Execute(appName, version);
		}

		private void Execute(string appName, string version)
		{
			var path = @GetProjectPath();
			var pipe = @path + "BuildPipeline/BuildPipeline.exe";

			var sb = new StringBuilder();
			sb.Append(" " + appName);
			sb.Append(" " + version);
			ExecuteCommand(pipe, sb.ToString());
		}

		private void ExecuteCommand(string exePath, string arguments)
		{
			var startInfo = new ProcessStartInfo
			{
				UseShellExecute = false,
				FileName = exePath,
				Arguments = arguments,
				CreateNoWindow = true,
				RedirectStandardOutput = true
			};

			var process = Process.Start(startInfo);
		}

		private string GetProjectPath()
		{
			var assetLength = "Assets".Length;
			return Application.dataPath.Remove(Application.dataPath.Length - assetLength);
		}
	}
}
#endif