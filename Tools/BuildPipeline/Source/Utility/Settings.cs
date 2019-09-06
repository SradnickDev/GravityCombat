using System.IO;
using Newtonsoft.Json;

namespace BuildPipeline
{
	public class Data
	{
		/// <summary>Path to Changelog Text File</summary>
		public string ChangelogFile = "D:/Folder/";

		/// <summary>Path to Source/Build Folder</summary>
		public string SourceFolder = "D:/FOLDER/";

		/// <summary>Path to Storage Folder</summary>
		public string StorageFolder = "D:/FOLDER/";

		/// <summary>ProfileName/ProjectName:Platform!</summary>
		public string ButlerTarget = "profile/gameName:platform";

		/// <summary>Folder ID from Google Drive</summary>
		public string DriveFolderID = "";

		/// <summary>Slack App/Bot Web Hook URL</summary>
		public string SlackWebHookUrl = "none";

		/// <summary>Slack Channel Name to post in</summary>
		public string SlackChannelName = "#none";

		/// <summary>User/Bot Name</summary>
		public string SlackUsername = "none";
	}

	public class Settings
	{
		private readonly string m_path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
		private bool m_dataAvailable = true;

		/// <summary>
		/// True if data file exists else false.
		/// </summary>
		/// <returns></returns>
		public bool DataAvailable()
		{
			return m_dataAvailable;
		}

		/// <summary>
		///  Data created from config file.
		/// </summary>
		/// <returns>Data from json</returns>
		public Data Get()
		{
			var retVal = Load();
			return retVal;
		}

		/// <summary>
		/// Load File, create one if no one exists.
		/// </summary>
		/// <returns>loaded config</returns>
		private Data Load()
		{
			if (!File.Exists(m_path))
			{
				Create();
				m_dataAvailable = false;
			}

			var text = File.ReadAllText(@m_path);
			var settings = JsonConvert.DeserializeObject<Data>(text);

			return settings;
		}

		/// <summary>
		/// Create a config file.
		/// </summary>
		private void Create()
		{
			var config = new Data();
			var jsonResult = JsonConvert.SerializeObject(config);

			using (var st = new StreamWriter(m_path, true))
			{
				st.WriteLine(jsonResult);
				st.Close();
			}
		}
	}
}