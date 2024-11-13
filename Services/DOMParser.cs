using System;
using System.Xml;

namespace DepartmentResourcesApp.Services
{
    public class DOMParser : IXmlParsingStrategy
    {
        private readonly Action<string> _outputAction;

        public DOMParser(Action<string> outputAction)
        {
            _outputAction = outputAction;
        }

        public void Parse(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList resources = doc.GetElementsByTagName("resource");

            foreach (XmlNode resource in resources)
            {
                if (resource.Attributes != null)
                {
                    foreach (XmlAttribute attr in resource.Attributes)
                    {
                        _outputAction($"{attr.Name}: {attr.Value}");
                    }
                }

                foreach (XmlNode child in resource.ChildNodes)
                {
                    _outputAction($"{child.Name}: {child.InnerText}");
                }
                _outputAction(new string('-', 30));
            }
        }

        public void SearchByKeyword(string filePath, string keyword)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList nodes = doc.GetElementsByTagName("resource");

            foreach (XmlNode node in nodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.InnerText.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        _outputAction($"Знайдено збіг: {child.Name} - {child.InnerText}");
                    }
                }
            }
        }
    }
}