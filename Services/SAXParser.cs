using System;
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
                        _outputAction($"Елемент: {reader.Name}");
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                _outputAction($"{reader.Name}: {reader.Value}");
                            }
                        }
                    }
                }
            }
        }

        public void SearchByKeyword(string filePath, string keyword)
        {
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text && reader.Value.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        _outputAction($"Знайдено збіг: {reader.Value}");
                    }
                }
            }
        }
    }
}
