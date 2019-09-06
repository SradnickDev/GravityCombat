using System.IO;
using System.IO.Compression;

namespace BuildPipeline
{
	public class Compress
	{
		/// <summary>
		/// Zip a files from a given Folder.
		/// Deletes existing Object.
		/// </summary>
		/// <param name="sourcePath">Folder to zip</param>
		/// <param name="targetPath">Target Folder</param>
		/// <param name="fileName">File name</param>
		/// <returns>created file path</returns>
		public string Folder(string @sourcePath, string @targetPath, string @fileName)
		{
			var compressedFile = targetPath + fileName + ".zip";

			if (File.Exists(compressedFile))
			{
				File.Delete(compressedFile);
			}

			if (Directory.Exists(sourcePath) && Directory.Exists(targetPath))
			{
				ZipFile.CreateFromDirectory(sourcePath, compressedFile, CompressionLevel.Fastest, false);
			}

			return compressedFile;
		}
	}
}