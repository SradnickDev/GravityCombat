using System;
using System.IO;
using System.Windows.Forms;

namespace BuildPipeline.Forms
{
	public partial class LogEditorWindow : Form
	{
		private string m_path = null;
		private string m_changeset = "";

		public string Changeset
		{
			get { return m_changeset; }
		}

		/// <summary>
		/// Simple Editor to enter a ChangeSet.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="version"></param>
		public LogEditorWindow(string path, string version)
		{
			m_path = path;
			InitializeComponent();
			SetVersion(version);
		}

		private void SetVersion(string version)
		{
			versionDisplay.Text = version;
		}

		private void saveBtn_Click(object sender, EventArgs e)
		{
			if (!File.Exists(m_path))
			{
				File.Create(m_path);
			}

			var fileContent = File.ReadAllText(m_path);
			File.WriteAllText(m_path, changeset.Text + "\n" + fileContent);
			m_changeset = changeset.Text;
		}

		private void closeBtn_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}