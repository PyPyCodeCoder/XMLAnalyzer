using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;

namespace Managers;

public class GoogleDriveManager
{
    protected const string AppDataFolderName = "appDataFolder";
    protected const string DefaultDataStoreFolderName = ".authdata";
    string _clientSecretsFilePath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), "client_secrets_xml_analyzer.json");
    
    protected static DriveService? DriveService;
    
    public bool IsClientLoggedIn = false;
    
    public async Task InitClient()
    {
        if (DriveService is not null)
        {
            return;
        }
        
        var clientSecrets = (await GoogleClientSecrets.FromFileAsync(_clientSecretsFilePath)).Secrets;
        var dataStore = new FileDataStore(DefaultDataStoreFolderName);
        dataStore.ClearAsync().Wait();
        var scopes = new[]
        {
            DriveService.Scope.DriveAppdata,
            DriveService.Scope.DriveFile
        };

        var credential =
            await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets
                , scopes
                , "user"
                , CancellationToken.None
                , dataStore);

        DriveService = new DriveService(new DriveService.Initializer
        {
            HttpClientInitializer = credential
        });
        
        IsClientLoggedIn = true;
    } 
    
    public async Task<List<(string, string)>> ListFiles()
    {
        if (DriveService is null)
        {
            throw new InvalidOperationException("Disk service is not initialized");
        }

        var request = DriveService.Files.List();
        request.Spaces = AppDataFolderName;
        request.Fields = "files(id, name)";

        var result = await request.ExecuteAsync();

        var list = new List<(string, string)>();

        foreach (var file in result.Files)
        {
            list.Add(($"{file.Name}", $"{file.Id}"));
        }

        return list;
    }
    
}