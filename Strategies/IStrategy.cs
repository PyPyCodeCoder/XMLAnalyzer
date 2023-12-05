using System.Xml;
using XMLData;

namespace Strategies;

public interface IStrategy
{
    public bool Load(Stream inpStream, XmlReaderSettings settings);
    
    public IList<Student> Search(SearchSettings settings);
}