using System.IO;

namespace Slave
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var path = Path.Combine(Directory.GetCurrentDirectory(), "slaveconfig.json");

			if (!SlackSlave.TryGetSettings(path, out var settings))
			{
				return;
			}

			var processInfo = HgHook.CreateProcessStartInfo();
			var result = HgHook.StartHgHook(processInfo);

			SlackSlave.SendSlackMessage(settings, result);
		}
	}
}