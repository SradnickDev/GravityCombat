using Slack.Webhooks;

namespace BuildPipeline.Services
{
	public class SlackButler
	{
		/// <summary>
		/// Creates a client to send a Message.
		/// </summary>
		/// <param name="data">Information needed to crate a Message</param>
		/// <param name="message">Will be posted.</param>
		public void SendMessage(Data data, string message)
		{
			var client = new SlackClient(data.SlackWebHookUrl, 100);

			var slackMessage = new SlackMessage()
			{
				Channel = data.SlackChannelName,
				Text = message,
				Username = data.SlackUsername
			};

			client.Post(slackMessage);
		}
	}
}