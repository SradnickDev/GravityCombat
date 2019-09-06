using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildPipeline.Forms
{
	public static class PopUp
	{
		/// <summary>
		/// Small Info popup, for eg. Warning, information.
		/// </summary>
		/// <param name="text">Content</param>
		/// <param name="caption">Header</param>
		/// <param name="asTask">True main Thread not be blocked, false blocks thread.</param>
		public static void Info(string text, string caption, bool asTask)
		{
			if (asTask)
			{
				Task.Run(() => { MessageBox.Show(text, caption, MessageBoxButtons.OK); });
			}
			else
			{
				MessageBox.Show(text, caption, MessageBoxButtons.OK);
			}
		}

		/// <summary>
		/// To Display Questions.
		/// Blocks current Thread
		/// </summary>
		/// <param name="text">Content</param>
		/// <param name="caption">Header</param>
		/// <returns>True if yes else False.</returns>
		public static bool YesNo(string text, string caption)
		{
			var result = MessageBox.Show(text, caption, MessageBoxButtons.YesNo);
			return result == DialogResult.Yes;
		}
	}
}