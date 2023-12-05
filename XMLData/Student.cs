namespace XMLData;

public class Student
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Faculty { get; set; }
    
    public string Department { get; set; }
    
    public string Course { get; set; }
    
    public DormAttributes DormAttributes { get; set; }

    public Student()
    {
        FirstName = "";
        LastName = "";
        Faculty = "";
        Department = "";
        Course = "";
        DormAttributes = new DormAttributes();
    }
}

public class DormAttributes
{
    public string RoomNumber { get; set; }
    
    public string CheckInDate { get; set; }
    
    public string CheckOutDate { get; set; }

    public DormAttributes()
    {
        RoomNumber = "";
        CheckInDate = "";
        CheckOutDate = "";
    }
}
