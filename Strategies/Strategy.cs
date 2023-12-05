using System.Xml;
using XMLData;

namespace Strategies;

public abstract class Strategy : IStrategy
{
    public IList<Student> Students;
    
    public abstract bool Load(Stream inpStream, XmlReaderSettings settings);
    
    public IList<Student> Search(SearchSettings settings)
    {
        return Students.Where(settings.CheckStudent).ToList();
    }
}