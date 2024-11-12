using System.Xml;

namespace DepartmentResourcesApp.Services
{
    public class SAXParser : IXmlParsingStrategy
    {
        private readonly Action<string> _outputAction;

        public SAXParser(Action<string> outputAction)
        {
            _outputAction = outputAction;
        }

        public void Parse(string filePath)
        {
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "resource")
                    {
                        string title = reader.GetAttribute("title");
                        string type = reader.GetAttribute("type");

                        _outputAction($"Назва: {title}");
                        _outputAction($"Тип: {type}");
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "annotation")
                        _outputAction($"Анотація: {reader.ReadElementContentAsString()}");

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "author")
                        _outputAction($"Автор: {reader.ReadElementContentAsString()}");

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "usage_conditions")
                        _outputAction($"Умови використання: {reader.ReadElementContentAsString()}");

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "address")
                        _outputAction($"Адреса: {reader.ReadElementContentAsString()}");
                }
            }
        }
    }
}
