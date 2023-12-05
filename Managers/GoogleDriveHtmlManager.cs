using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Managers;

public class GoogleDriveHtmlManager : GoogleDriveManager
{
    public async Task<string> SaveFile(string fileName, FileResult file)
    {
        if (DriveService is null)
        {
            throw new InvalidOperationException("Disk service is not initialized");
        }

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = $"{fileName}.html",
            Parents = new List<string>()
            {
                AppDataFolderName
            }
        };

        var str = await file.OpenReadAsync();

        string htmlContent = string.Empty;

        using (StreamReader streamReader = new StreamReader(str))
        {
            htmlContent = streamReader.ReadToEnd();
        }
        
        string transformedHtml = ApplyXslTransformation(htmlContent);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(transformedHtml));
        var request = DriveService.Files.Create(fileMetadata, stream, "text/html");
        request.Fields = "id";

        var uploadResult = await request.UploadAsync();

        if (uploadResult.Exception is not null)
        {
            throw new Exception($"Error while uploading a file: {uploadResult.Exception.Message}");
        }

        return request.ResponseBody.Id;
    }
    
    private string ApplyXslTransformation(string htmlContent)
    {
        var xslTransform = new XslCompiledTransform();
        xslTransform.Load(GlobalVariable.GlobalXsltPath);

        using var transformedStream = new MemoryStream();
        using (var xmlWriter = XmlWriter.Create(transformedStream, xslTransform.OutputSettings))
        {
            using (var htmlReader = new StringReader(htmlContent))
            {
                using (var xmlReader = XmlReader.Create(htmlReader))
                {
                    xslTransform.Transform(xmlReader, xmlWriter);
                }
            }
        }

        transformedStream.Position = 0;
        using var transformedReader = new StreamReader(transformedStream);
        return transformedReader.ReadToEnd();
    }
}

