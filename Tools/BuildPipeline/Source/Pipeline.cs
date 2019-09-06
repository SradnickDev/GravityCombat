using BuildPipeline.Forms;
using BuildPipeline.Services;

namespace BuildPipeline
{
	public class Pipeline
	{
		private Settings m_settings;
		private Compress m_compress;
		private ItchButler m_itchButler;
		private DriveButler m_driveButler;
		private SlackButler m_slackButler;

		public Pipeline()
		{
			m_settings = new Settings();
			m_compress = new Compress();
			m_itchButler = new ItchButler();
			m_driveButler = new DriveButler();
			m_slackButler = new SlackButler();
		}

		public void Start(string projectName, string version)
		{
			if (projectName == string.Empty || version == string.Empty)
			{
				PopUp.Info("No  arguments were given!\n" +
							"arguments : projectName version",
					"Warning!", false);
				return;
			}

			if (TryGetData(out var data))
			{
				return;
			}


			var file = CompressFile(projectName, version, data);


			ItchUpload(version, file, data);


			var result = m_driveButler.Upload(file, data.DriveFolderID);
			if (!result) return;


			var window = new LogEditorWindow(data.ChangelogFile, version);
			window.ShowDialog();

			m_slackButler.SendMessage(data, window.Changeset);
		}

		/// <summary>
		/// Wraps Itch.IO Butler
		/// </summary>
		/// <param name="version">Build Version</param>
		/// <param name="file">Zip File</param>
		/// <param name="data">Data generated from config file</param>
		private void ItchUpload(string version, string file, Data data)
		{
			m_itchButler.Upload(file, data.ButlerTarget, version);
		}

		/// <summary>
		/// Wraps Compression.
		/// </summary>
		/// <param name="projectName">Name for result file</param>
		/// <param name="version">add for result file</param>
		/// <param name="data">Data generated from config file</param>
		/// <returns>Path to zipped FIle</returns>
		private string CompressFile(string projectName, string version, Data data)
		{
			var file = m_compress.Folder(data.SourceFolder, data.StorageFolder, projectName + version);
			return file;
		}

		private bool TryGetData(out Data data)
		{
			data = m_settings.Get();

			if (!m_settings.DataAvailable())
			{
				PopUp.Info("Config.json not found. Template created, please fill!",
					"Warning!", false);
				return true;
			}

			return false;
		}
	}
}