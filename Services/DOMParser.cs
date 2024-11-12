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
                var title = resource.Attributes["title"]?.InnerText;
                var type = resource.Attributes["type"]?.InnerText;

                _outputAction($"Назва: {title}");
                _outputAction($"Тип: {type}");

                foreach (XmlNode child in resource.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "annotation":
                            _outputAction($"Анотація: {child.InnerText}");
                            break;
                        case "author":
                            _outputAction($"Автор: {child.InnerText}");
                            break;
                        case "usage_conditions":
                            _outputAction($"Умови використання: {child.InnerText}");
                            break;
                        case "address":
                            _outputAction($"Адреса: {child.InnerText}");
                            break;
                    }
                }
                _outputAction(new string('-', 30));
            }
        }
    }
}
