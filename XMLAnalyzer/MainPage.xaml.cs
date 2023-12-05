using System.Text;
using System.Xml;
using System.Xml.Xsl;
using CommunityToolkit.Maui.Storage;
using Managers;
using Strategies;
using EventLogger;
using XMLData;

namespace XMLAnalyzer;

public partial class MainPage : ContentPage
{
    private IStrategy _strategy;
    
    private FileResult _chosenFile;
    
    private XmlReaderSettings _xmlRSettings;
    
    private XslCompiledTransform _transformer;
    
    private IFileSaver _fileSaver;
    private IFilePicker _filePicker;
    
    private GoogleDriveManager _googleDriveManager;
    
    private EventLogger.EventLogger _logger;
    
    public MainPage()
    {
        InitializeComponent();
        
        _fileSaver = FileSaver.Default;
        _filePicker = FilePicker.Default;
        
        _googleDriveManager = new GoogleDriveManager();
        
        StrategyPicker.SelectedIndex = 0;
        
        _logger = EventLogger.EventLogger.Instance;
    }

    private async void OpenButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (_xmlRSettings == null)
            {
                string xsdFilePath = "D:\\Valera\\122_22_2\\OOP\\XMLAnalyzer\\XMLData\\Students.xsd";
                
                ImportSchema(xsdFilePath);
            }
        
            var options = new PickOptions
            {
                PickerTitle = "Select .xml file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".xml" } },
                })
            };
        
            _chosenFile = await _filePicker.PickAsync(options);
        
            await CheckFile();
        
            _logger.LogEvent(LogLevel.Default, $"Opened file {_chosenFile.FileName}");
            await DisplayAlert("Success", "File opened successfully", "Ок");
        }
        catch (Exception ex)
        {
            _logger.LogEvent(LogLevel.Default, $"Error: {ex.Message}");
            await DisplayAlert("Error", $"{ex.Message}", "Oк");
        }
    }
    
    private void Selected_Strategy(object sender, EventArgs e)
    {
        switch (StrategyPicker.SelectedIndex)
        {
            case 0:
                _strategy = new SAXStrategy();
                break;
            case 1:
                _strategy = new DOMStrategy();
                break;
            case 2:
                _strategy = new LINQStrategy();
                break;
        }
    }
    
    private async void SearchButton_Clicked(object sender, EventArgs e)
    {
        if (_chosenFile == null)
        {
            await DisplayAlert("Error", "Input file is not chosen", "Ok");
            _logger.LogEvent(LogLevel.Default, "Input file is not chosen");
            return;
        }

        if (_strategy == null)
        {
            await DisplayAlert("Error", "Parser type is not chosen", "Ok");
            _logger.LogEvent(LogLevel.Default, "Parser type is not chosen");
            return;
        }

        var searchSettings = GetSearchSettings();
        ClearResults();

        _logger.LogEvent(LogLevel.Filter, $"Filter Parameters: {searchSettings.GetSearchSettingsLogMessage()}");
        
        var results = _strategy.Search(searchSettings);
        for (int i = 1; i <= results.Count; ++i)
        {
            DisplayResult(results[i - 1], i);
        }
    }
    
    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        ClearSearchSettings();
    }
    
    private async void SaveHtmlButton_Clicked(object sender, EventArgs e)
    {
        if (_chosenFile == null)
        {
            await DisplayAlert("Error", "Input file is not chosen", "Ok");
            return;
        }
        
        _transformer = new();
        _transformer.Load(GlobalVariable.GlobalXsltPath);

        using var stream = new MemoryStream(Encoding.Default.GetBytes(""));
        
        var result = await _fileSaver.SaveAsync(_chosenFile.FileName.Split(".")[0] + ".html", stream, new CancellationTokenSource().Token);
        
        _transformer.Transform(_chosenFile.FullPath, result.FilePath);
        
        await DisplayAlert("Success", "File was saved successfully", "Ok");
        
        _logger.LogEvent(LogLevel.Transform, $"File {_chosenFile.FileName} was transformed successfully");
    }
    
    private async void ExitButton_Clicked(object sender, EventArgs e)
    {
        var answer = await DisplayAlert("Підтвердження", "Чи дійсно ви хочете завершити роботу з програмою?",
            "Так", "Ні");
        if (answer)
        {
            _logger.LogEvent(LogLevel.Default, "Exiting the program");
            Environment.Exit(0);
        }
    }

    
    private async void GetXmlGDriveButton_Clicked(object sender, EventArgs e)
    {
        if (!_googleDriveManager.IsClientLoggedIn)
        {
            try
            {
                await _googleDriveManager.InitClient();
                await DisplayAlert("Success", "Login successful", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while logging in:", $"{ex.Message}", "Ok");
            }
        }
        
        var filesList = await _googleDriveManager.ListFiles();
        var files = string.Join("", filesList);
    
        await DisplayAlert("Files in Google Drive:", $"{files}", "Ok");
        
        var idList = new List<string>();
        foreach (var file in filesList)
        {
            idList.Add(file.Item2);
        }
        
        var fileId = await DisplayActionSheet("Select the .xml file to upload", "Cancel", null, idList.ToArray());
    
        if (string.IsNullOrWhiteSpace(fileId))
        {
            await DisplayAlert("Error", "File Id is not entered", "Ok");
        }
        else
        {
            try
            {
                var GDXMLManager = new GoogleDriveXmlManager();
                
                var content = await GDXMLManager.GetFile(fileId);
                
                using var stream = new MemoryStream(Encoding.Default.GetBytes(content));
                
                _chosenFile = new FileResult((await _fileSaver.SaveAsync("example.xml", stream, new CancellationTokenSource().Token)).FilePath);
                
                await DisplayAlert("Success", "File was successfully uploaded", "Ok");
                
                _logger.LogEvent(LogLevel.Default, $"Opened file {_chosenFile.FileName}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while uploading a file:", $"{ex.Message}", "Ok");
            }
        }
    }
    
    private async void SaveXmlGDriveButton_Clicked(object sender, EventArgs e)
    {
        if (!_googleDriveManager.IsClientLoggedIn)
        {
            try
            {
                await _googleDriveManager.InitClient();
                await DisplayAlert("Success", "Login successful", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while logging in:", $"{ex.Message}", "Ok");
            }
        }
        
        string fileName = await DisplayPromptAsync("File name", "Enter the name of the file to save:");
        
        if (string.IsNullOrWhiteSpace(fileName))
        {
            await DisplayAlert("Error", "Incorrect file name", "Ok");
        }
        else
        {
            try
            {
                var GDXMLManager = new GoogleDriveXmlManager();
                
                string id = await GDXMLManager.SaveFile(fileName, _chosenFile);
                
                await DisplayAlert("Success", $"Created file with id: {id}", "Ok");
                
                _logger.LogEvent(LogLevel.Save, $"File {_chosenFile.FileName} was saved successfully");
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while saving the file:", $"{ex.Message}", "Ok");
            }
        }
    }
    
    private async void SaveHtmlGDriveButton_Clicked(object sender, EventArgs e)
    {
        if (!_googleDriveManager.IsClientLoggedIn)
        {
            try
            {
                await _googleDriveManager.InitClient();
                await DisplayAlert("Success", "Login successful", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while logging in:", $"{ex.Message}", "Ok");
            }
        }
        
        string fileName = await DisplayPromptAsync("File name", "Enter the name of the file to save:");
        
        if (string.IsNullOrWhiteSpace(fileName))
        {
            await DisplayAlert("Error", "Incorrect file name", "Ok");
        }
        else
        {
            try
            {
                var GDHTMLManager = new GoogleDriveHtmlManager();
                
                string id = await GDHTMLManager.SaveFile(fileName, _chosenFile);
                
                await DisplayAlert("Success", $"Created file with id: {id}", "Ok");
                
                _logger.LogEvent(LogLevel.Transform, $"File {_chosenFile.FileName} was transformed successfully");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while saving the file:", $"{ex.Message}", "Ok");
            }
        }
    }
    
    private async void GetXsltGDriveButton_Clicked(object sender, EventArgs e)
    {
        if (!_googleDriveManager.IsClientLoggedIn)
        {
            try
            {
                await _googleDriveManager.InitClient();
                await DisplayAlert("Success", "Login successful", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while logging in:", $"{ex.Message}", "Ok");
            }
        }
        
        var filesList = await _googleDriveManager.ListFiles();
        var files = string.Join("", filesList);
    
        await DisplayAlert("Files in Google Drive:", $"{files}", "Ok");
        
        var idList = new List<string>();
        foreach (var file in filesList)
        {
            idList.Add(file.Item2);
        }
        
        var fileId = await DisplayActionSheet("Select the .xsl file to upload", "Cancel", null, idList.ToArray());
    
        if (string.IsNullOrWhiteSpace(fileId))
        {
            await DisplayAlert("Error", "File Id is not entered", "Ok");
        }
        else
        {
            try
            {
                var GDXSLTManager = new GoogleDriveXsltManager();
                
                var content = await GDXSLTManager.GetFile(fileId);
                
                using var stream = new MemoryStream(Encoding.Default.GetBytes(content));
                
                var newFile = new FileResult((await _fileSaver.SaveAsync("example.xsl", stream, new CancellationTokenSource().Token)).FilePath);

                
                GlobalVariable.GlobalXsltPath = newFile.FullPath;
                
                
                await DisplayAlert("Success", "File was successfully uploaded", "Ok");
                
                _logger.LogEvent(LogLevel.Default, $"Opened file {newFile.FileName}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while uploading a file:", $"{ex.Message}", "Ok");
            }
        }
    }

    private async void SaveXsltGDriveButton_Clicked(object sender, EventArgs e)
    {
        if (!_googleDriveManager.IsClientLoggedIn)
        {
            try
            {
                await _googleDriveManager.InitClient();
                await DisplayAlert("Success", "Login successful", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error while logging in:", $"{ex.Message}", "Ok");
            }
        }
        
        try
        {
            var GDXSLTManager = new GoogleDriveXsltManager();
            
            var options = new PickOptions
            {
                PickerTitle = "Select .xsl file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".xsl" } },
                })
            };
    
            FileResult newFile = await _filePicker.PickAsync(options);
            
            string id = await GDXSLTManager.SaveFile(newFile.FileName, newFile);
            
            await DisplayAlert("Success", $"Created file with id: {id}", "Ok");
            
            _logger.LogEvent(LogLevel.Save, $"File {newFile.FileName} was saved successfully");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error while saving the file:", $"{ex.Message}", "Ok");
        }
    }
    
}