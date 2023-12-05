using System.Xml;
using System.Xml.Schema;
using XMLData;

namespace XMLAnalyzer;

public partial class MainPage
{
    private async Task CheckFile()
    {
        if (_strategy == null || _chosenFile == null)
        {
            return;
        }
    
        try
        {
            if (_strategy.Load(await _chosenFile.OpenReadAsync(), _xmlRSettings))
            {
                return;
            }
            
            _chosenFile = null;
            await DisplayAlert("Error", "File can't be read", "Ok");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during file check: {ex.Message}");
        }
    }
    
    private void ImportSchema(string filepath)
    {
        try
        {
            var schema = new XmlSchemaSet();
            schema.Add(null, filepath);

            _xmlRSettings = new XmlReaderSettings
            {
                Schemas = schema,
                ValidationType = ValidationType.Schema
            };

            _xmlRSettings.ValidationEventHandler += (object? sender, ValidationEventArgs e) =>
            {
                if (e.Severity == XmlSeverityType.Error) throw new Exception();
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during schema import: {ex.Message}");
        }
    }
    
    private void ClearResults()
    {
        while (ResultsTable.Children.Count > 8)
        {
            ResultsTable.Children.RemoveAt(8);
        }
        while (ResultsTable.RowDefinitions.Count > 1)
        {
            ResultsTable.RowDefinitions.RemoveAt(1);
        }
    }
    
    private void DisplayResult(Student student, int row)
    {
        ResultsTable.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        
        CreateLabel(row, 0, student.FirstName);
        CreateLabel(row, 1, student.LastName);
        CreateLabel(row, 2, student.Faculty);
        CreateLabel(row, 3, student.Department);
        CreateLabel(row, 4, student.Course);
        CreateLabel(row, 5, student.DormAttributes.RoomNumber);
        CreateLabel(row, 6, student.DormAttributes.CheckInDate);
        CreateLabel(row, 7, student.DormAttributes.CheckOutDate);
    }
    
    private void CreateLabel(int row, int column, string text)
    {
        var label = new Label
        {
            Text = text,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        ResultsTable.SetRow(label, row);
        ResultsTable.SetColumn(label, column);
        ResultsTable.Children.Add(label);
    }
    
    private SearchSettings GetSearchSettings()
    {
        var settings = new SearchSettings();
        
        if (FirstNameCheckbox.IsChecked)
        {
            settings.FirstName = FirstNameEntry.Text ?? "";
        }

        if (LastNameCheckbox.IsChecked)
        {
            settings.LastName = LastNameEntry.Text ?? "";
        }

        if (FacultyCheckbox.IsChecked)
        {
            settings.Faculty = FacultyEntry.Text ?? "";
        }

        if (DepartmentCheckbox.IsChecked)
        {
            settings.Department = DepartmentEntry.Text ?? "";
        }

        if (CourseCheckbox.IsChecked)
        {
            settings.Course = CourseEntry.Text ?? "";
        }

        if (RoomNumberCheckbox.IsChecked)
        {
            settings.RoomNumber = RoomNumberEntry.Text ?? "";
        }

        if (CheckInDateCheckbox.IsChecked)
        {
            settings.CheckInDate = CheckInDateEntry.Text ?? "";
        }

        if (CheckOutDateCheckbox.IsChecked)
        {
            settings.CheckOutDate = CheckOutDateEntry.Text ?? "";
        }

        return settings;
    }
    
    private void ClearSearchSettings()
    {
        FirstNameEntry.Text = "";
        LastNameEntry.Text = "";
        FacultyEntry.Text = "";
        DepartmentEntry.Text = "";
        CourseEntry.Text = "";

        RoomNumberEntry.Text = "";
        CheckInDateEntry.Text = "";
        CheckOutDateEntry.Text = "";

        FirstNameCheckbox.IsChecked = false;
        LastNameCheckbox.IsChecked = false;
        FacultyCheckbox.IsChecked = false;
        DepartmentCheckbox.IsChecked = false;
        CourseCheckbox.IsChecked = false;

        RoomNumberCheckbox.IsChecked = false;
        CheckInDateCheckbox.IsChecked = false;
        CheckOutDateCheckbox.IsChecked = false;
    }
    
}
