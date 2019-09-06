using System.IO;
using System.Threading;
using System.Collections.Generic;
using BuildPipeline.Forms;
using Google.Apis.Upload;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using File = Google.Apis.Drive.v3.Data.File;

namespace BuildPipeline.Services
{
	public class DriveButler
	{
		private const string ApplicationName = "DriveButler";

		private readonly string[] m_scopes = {DriveService.Scope.Drive};
		private UserCredential m_credential = null;

		/// <summary>
		/// Upload a file to a Google drive directory.
		/// </summary>
		/// <param name="pathToFile"></param>
		/// <param name="driverParentId"></param>
		public bool Upload(string pathToFile, string driverParentId)
		{
			GetCredential();
			var service = CreateDriveService();

			var result = UploadFile(service, pathToFile, driverParentId);
			var retVal = OnProgressChanged(result);
			return retVal;
		}

		private void GetCredential()
		{
			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				const string credPath = "token.json";

				m_credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					m_scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
			}
		}

		private DriveService CreateDriveService()
		{
			var service = new DriveService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = m_credential,
				ApplicationName = ApplicationName
			});
			return service;
		}

		private IUploadProgress UploadFile(DriveService driveService, string pathToFile, string driverParentId)
		{
			var fileMetadata = new File
			{
				Name = Path.GetFileName(pathToFile),
				MimeType = "application/zip",
				Parents = new List<string>() {driverParentId}
			};

			FilesResource.CreateMediaUpload request;


			using (var stream = new FileStream(pathToFile, FileMode.Open))
			{
				request = driveService.Files.Create(fileMetadata, stream, "application/zip");
				request.Fields = "id";
				return request.Upload();
			}
		}

		private bool OnProgressChanged(IUploadProgress progress)
		{
			switch (progress.Status)
			{
				case UploadStatus.NotStarted:
					PopUp.Info("Google Drive upload failed !", "Error!", false);
					return false;

				case UploadStatus.Completed:
					PopUp.Info("Google Drive upload completed", "Info!", false);
					return true;

				case UploadStatus.Failed:
					PopUp.Info("Google Drive upload failed", "Error!", true);
					return false;

				default:
					return false;
			}
		}
	}
}