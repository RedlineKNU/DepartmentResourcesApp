using System;
using System.Linq;
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
            XDocument doc = XDocument.Load(filePath);

            var resources = doc.Descendants("resource");

            foreach (var resource in resources)
            {
                foreach (var attr in resource.Attributes())
                {
                    _outputAction($"{attr.Name}: {attr.Value}");
                }

                foreach (var element in resource.Elements())
                {
                    _outputAction($"{element.Name}: {element.Value}");
                }
                _outputAction(new string('-', 30));
            }
        }

        public void SearchByKeyword(string filePath, string keyword)
        {
            XDocument doc = XDocument.Load(filePath);

            var matches = doc.Descendants().Where(x => x.Value.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            foreach (var match in matches)
            {
                _outputAction($"Знайдено збіг: {match.Name} - {match.Value}");
            }
        }
    }
}
