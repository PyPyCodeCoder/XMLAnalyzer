using System.Xml;
using System.Xml.Linq;
using XMLData;

namespace Strategies;

public class LINQStrategy : Strategy
{
    public LINQStrategy()
    {
        Students = new List<Student>();
    }

    public override bool Load(Stream inpStream, XmlReaderSettings settings)
    {
        try
        {
            Students.Clear();
            
            using (var xmlReader = XmlReader.Create(inpStream, settings))
            {
                var doc = XDocument.Load(xmlReader);

                Students = doc.Descendants("Student")
                    .Select(s => new Student
                    {
                        FirstName = s.Element("FirstName")?.Value ?? "",
                        LastName = s.Element("LastName")?.Value ?? "",
                        Faculty = s.Element("Faculty")?.Value ?? "",
                        Department = s.Element("Department")?.Value ?? "",
                        Course = s.Element("Course")?.Value ?? "",
                        DormAttributes = new DormAttributes
                        {
                            RoomNumber = s.Element("DormAttributes")?.Element("RoomNumber")?.Value ?? "",
                            CheckInDate = s.Element("DormAttributes")?.Element("CheckInDate")?.Value ?? "",
                            CheckOutDate = s.Element("DormAttributes")?.Element("CheckOutDate")?.Value ?? "",
                        }
                    })
                    .ToList();

                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}