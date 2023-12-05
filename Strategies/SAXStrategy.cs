using System.Xml;
using XMLData;

namespace Strategies;

public class SAXStrategy : Strategy
{
    public SAXStrategy()
    {
        Students = new List<Student>();
    }
    
    public override bool Load(Stream inpStream, XmlReaderSettings settings)
    {
        Students.Clear();

        try
        {
            var reader = XmlReader.Create(inpStream, settings);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Student")
                {
                    Student student = ParseStudent(reader);
                    Students.Add(student);
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    private Student ParseStudent(XmlReader reader)
    {
        var student = new Student();

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "FirstName":
                        student.FirstName = ReadElementText(reader);
                        break;
                    case "LastName":
                        student.LastName = ReadElementText(reader);
                        break;
                    case "Faculty":
                        student.Faculty = ReadElementText(reader);
                        break;
                    case "Department":
                        student.Department = ReadElementText(reader);
                        break;
                    case "Course":
                        student.Course = ReadElementText(reader);
                        break;
                    case "DormAttributes":
                        student.DormAttributes = ParseDormAttributes(reader);
                        break;
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Student")
            {
                break;
            }
        }

        return student;
    }

    private DormAttributes ParseDormAttributes(XmlReader reader)
    {
        var dormAttributes = new DormAttributes();

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "RoomNumber":
                        dormAttributes.RoomNumber = ReadElementText(reader);
                        break;
                    case "CheckInDate":
                        dormAttributes.CheckInDate = ReadElementText(reader);
                        break;
                    case "CheckOutDate":
                        dormAttributes.CheckOutDate = ReadElementText(reader);
                        break;
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "DormAttributes")
            {
                break;
            }
        }

        return dormAttributes;
    }

    private string ReadElementText(XmlReader reader)
    {
        if (reader.Read())
        {
            return reader.Value.Trim();
        }
        throw new Exception("Error reading XML element text.");
    }
    
}