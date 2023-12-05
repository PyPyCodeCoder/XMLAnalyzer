namespace XMLData;

public class SearchSettings
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Faculty { get; set; }
    
    public string Department { get; set; }
    
    public string Course { get; set; }
    
    public string RoomNumber { get; set; }
    
    public string CheckInDate { get; set; }
    
    public string CheckOutDate { get; set; }
    
    public SearchSettings()
    {
        FirstName = "";
        LastName = "";
        Faculty = "";
        Department = "";
        Course = "";
        RoomNumber = "";
        CheckInDate = "";
        CheckOutDate = "";
    }
    
    public bool CheckStudent(Student student)
    {
        var firstName = student.FirstName.ToLower().Contains(FirstName.ToLower());
        
        var lastName = student.LastName.ToLower().Contains(LastName.ToLower());
        
        var faculty = student.Faculty.ToLower().Contains(Faculty.ToLower());
        
        var department = student.Department.ToLower().Contains(Department.ToLower());
        
        var course = student.Course.ToLower().Contains(Course.ToLower());
        
        var roomNumber = student.DormAttributes.RoomNumber.ToLower().Contains(RoomNumber.ToLower());
        
        var checkInDate = student.DormAttributes.CheckInDate.ToLower().Contains(CheckInDate.ToLower());
        
        var checkOutDate = student.DormAttributes.CheckOutDate.ToLower().Contains(CheckOutDate.ToLower());

        
        return firstName && lastName && faculty && department && course && roomNumber && checkInDate && checkOutDate;
    }
    
    public string GetSearchSettingsLogMessage()
    {
        var logMessage = $"FirstName: {FirstName}, LastName: {LastName}, " +
                         $"Faculty: {Faculty}, Department: {Department}, " +
                         $"Course: {Course}, RoomNumber: {RoomNumber}, " +
                         $"CheckInDate: {CheckInDate}, CheckOutDate: {CheckOutDate}";

        return logMessage;
    }
}