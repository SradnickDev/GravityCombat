using System.Diagnostics;

namespace Slave
{
	public class HgHook
	{
		
		/// <summary>
		/// Create default Process info, with Arguments.
		/// </summary>
		/// <returns>Process infos</returns>
		public static ProcessStartInfo CreateProcessStartInfo()
		{
			var processInfo = new ProcessStartInfo
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				FileName = "hg.exe",
				RedirectStandardError = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				Arguments =
					"log -r . --template \"" +
					"Author: {author}\n " +
					"Branch : {branch} \n" +
					"Date : {date|isodate} \n" +
					"Commit Msg :\n {desc} \n" +
					"######################################\n"
			};
			return processInfo;
		}

		/// <summary>
		/// Creates a Process .
		/// </summary>
		/// <param name="processInfo">Hg process infos</param>
		/// <returns>Message from that was requested in processInfo</returns>
		public static string StartHgHook(ProcessStartInfo processInfo)
		{
			var result = "";
			using (var process = Process.Start(processInfo))
			{
				using (var reader = process?.StandardOutput)
				{
					result += reader?.ReadToEnd();
				}
			}

			return result;
		}
	}
}