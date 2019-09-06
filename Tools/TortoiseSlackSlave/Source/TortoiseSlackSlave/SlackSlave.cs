using System.IO;
using Slack.Webhooks;
using Newtonsoft.Json;

namespace Slave
{
	public class SlackSettings
	{
		public string WebHookUrl = "none";
		public string ChannelName = "none";
		public string Username = "none";
	}

	public class SlackSlave
	{
		/// <summary>
		/// If file exists return true else false.
		/// </summary>
		/// <param name="path">Path to Json File</param>
		/// <param name="settings">Slack Settings</param>
		/// <returns></returns>
		public static bool TryGetSettings(string path, out SlackSettings settings)
		{
			if (File.Exists(path))
			{
				settings = ReadSettings(path);
				return true;
			}

			settings = new SlackSettings();
			CreateFile(settings, path);
			return false;
		}

		/// <summary>
		/// Deserialize Json File to Slack Settings
		/// </summary>
		/// <param name="path">Path to json File.</param>
		/// <returns></returns>
		private static SlackSettings ReadSettings(string path)
		{
			var text = File.ReadAllText(@path);
			var settings = JsonConvert.DeserializeObject<SlackSettings>(text);
			return settings;
		}

		/// <summary>
		/// Creates json file at given Path.
		/// </summary>
		/// <param name="settings">Default Settings</param>
		/// <param name="path">Path to no yet existing File.</param>
		private static void CreateFile(SlackSettings settings, string path)
		{
			var jsonResult = JsonConvert.SerializeObject(settings);

			using (var st = new StreamWriter(path, true))
			{
				st.WriteLine(jsonResult);
				st.Close();
			}
		}

		/// <summary>
		/// Creates a Slack Client to send a message to Slack.
		/// </summary>
		/// <param name="settings">Contains information to send a valid Message</param>
		/// <param name="message">Message that will be send to slack.</param>^
		public static void SendSlackMessage(SlackSettings settings, string message)
		{
			var slackClient =
				new SlackClient(settings.WebHookUrl);

			var slackMessage = new SlackMessage
			{
				Channel = settings.ChannelName,
				Text = message,
				Username = settings.Username
			};
			slackClient.Post(slackMessage);
		}
	}
}