using BuildPipeline.Forms;
using BuildPipeline.Services;

namespace BuildPipeline
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			var m_pipeline = new Pipeline();

			var projectName = args[0];
			var version = args[1];
			m_pipeline.Start(projectName, version);
		}
	}
}