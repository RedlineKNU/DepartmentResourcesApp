using System.Xml.Linq;

namespace DepartmentResourcesApp.Services
{
    public class LINQParser : IXmlParsingStrategy
    {
        private readonly Action<string> _outputAction;

        public LINQParser(Action<string> outputAction)
        {
            _outputAction = outputAction;
        }

        public void Parse(string filePath)
        {
            XDocument document = XDocument.Load(filePath);

            var resources = from resource in document.Descendants("resource")
                            select new
                            {
                                Title = resource.Attribute("title")?.Value,
                                Type = resource.Attribute("type")?.Value,
                                Annotation = resource.Element("annotation")?.Value,
                                Author = resource.Element("author")?.Value,
                                UsageConditions = resource.Element("usage_conditions")?.Value,
                                Address = resource.Element("address")?.Value
                            };

            foreach (var resource in resources)
            {
                _outputAction($"Назва: {resource.Title}");
                _outputAction($"Тип: {resource.Type}");
                _outputAction($"Анотація: {resource.Annotation}");
                _outputAction($"Автор: {resource.Author}");
                _outputAction($"Умови використання: {resource.UsageConditions}");
                _outputAction($"Адреса: {resource.Address}");
                _outputAction(new string('-', 30));
            }
        }
    }
}
