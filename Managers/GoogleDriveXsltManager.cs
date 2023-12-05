using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Storage;

namespace Managers
{
    public class GoogleDriveXsltManager : GoogleDriveManager
    {
        public async Task<string> GetFile(string fileId)
        {
            if (DriveService == null)
            {
                throw new InvalidOperationException("Disk service is not initialized");
            }

            var request = DriveService.Files.Get(fileId);

            using var stream = new MemoryStream();

            try
            {
                await request.DownloadAsync(stream);

                if (stream == null || stream.Length == 0)
                {
                    throw new Exception("Downloaded stream is null or empty.");
                }

                stream.Position = 0;

                using var streamReader = new StreamReader(stream);
                string fileContent = await streamReader.ReadToEndAsync();

                return fileContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while processing a file: {ex.Message}", ex);
            }
        }

        public async Task<string> SaveFile(string fileName, FileResult file)
        {
            if (DriveService is null)
            {
                throw new InvalidOperationException("Disk service is not initialized");
            }

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = $"{fileName}",
                Parents = new List<string>()
                {
                    AppDataFolderName
                }
            };

            var str = await file.OpenReadAsync();

            string xsltContent = string.Empty;

            using (StreamReader streamReader = new StreamReader(str))
            {
                xsltContent = streamReader.ReadToEnd();
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xsltContent));
            var request = DriveService.Files.Create(fileMetadata, stream, "application/xsl");
            request.Fields = "id";

            var uploadResult = await request.UploadAsync();

            if (uploadResult.Exception is not null)
            {
                throw new Exception($"Error while uploading a file: {uploadResult.Exception.Message}");
            }

            return request.ResponseBody.Id;
        }
    }
}

public class GlobalVariable
{
    public static string GlobalXsltPath = "D:\\Valera\\122_22_2\\OOP\\XMLAnalyzer\\XMLData\\Default.xsl";
}
