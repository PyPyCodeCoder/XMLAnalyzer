using System.Xml;
using XMLData;

namespace Strategies;

public class DOMStrategy : Strategy
{
    public DOMStrategy()
    {
        Students = new List<Student>();
    }
    
    public override bool Load(Stream inpStream, XmlReaderSettings settings)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(inpStream);
            
            Students.Clear();
            
            foreach (XmlNode studentNode in xmlDoc.SelectNodes("/StudentsList/Student"))
            {
                Student student = new Student();
                
                student.FirstName = studentNode.SelectSingleNode("FirstName").InnerText;
                student.LastName = studentNode.SelectSingleNode("LastName").InnerText;
                student.Faculty = studentNode.SelectSingleNode("Faculty").InnerText;
                student.Department = studentNode.SelectSingleNode("Department").InnerText;
                student.Course = studentNode.SelectSingleNode("Course").InnerText;
                
                XmlNode dormNode = studentNode.SelectSingleNode("DormAttributes");
                student.DormAttributes.RoomNumber = dormNode.SelectSingleNode("RoomNumber").InnerText;
                student.DormAttributes.CheckInDate = dormNode.SelectSingleNode("CheckInDate").InnerText;
                student.DormAttributes.CheckOutDate = dormNode.SelectSingleNode("CheckOutDate").InnerText;

                Students.Add(student);
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}