using System.Diagnostics;
using BuildPipeline.Forms;

namespace BuildPipeline.Services
{
	public class ItchButler
	{
		public void Upload(string pathToFile, string butlerTarget, string version)
		{
			var processInfo = CreateProcessStartInfo(pathToFile, butlerTarget, version);
			var processRetVal = StartProcess(processInfo);
		}

		/// <summary>
		/// Create default Process info, with Arguments.
		/// </summary>
		/// <returns>Process infos</returns>
		private ProcessStartInfo CreateProcessStartInfo(string pathToFile, string butlerTarget, string version)
		{
			var processInfo = new ProcessStartInfo
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				FileName = "powershell.exe",
				RedirectStandardError = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				Arguments = $"butler push {pathToFile} {butlerTarget} --userversion {version}"
			};
			return processInfo;
		}

		/// <summary>
		/// Starts a Process.
		/// </summary>
		/// <param name="processInfo">butler process infos</param>
		/// <returns>butler information</returns>
		private string StartProcess(ProcessStartInfo processInfo)
		{
			var result = "";
			using (var process = Process.Start(processInfo))
			{
				using (var reader = process?.StandardOutput)
				{
					result += reader?.ReadToEnd();
				}
			}


			PopUp.Info("Itch.Io Upload Completed.", "Info!", true);
			
			return result;
		}
	}
}