using System;
using System.Linq;
using System.Xml.Linq;

namespace DepartmentResourcesApp.Services
{
    public class LINQParser : IXmlParsingStrategy
    {
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
                Console.WriteLine($"Назва: {resource.Title}");
                Console.WriteLine($"Тип: {resource.Type}");
                Console.WriteLine($"Анотація: {resource.Annotation}");
                Console.WriteLine($"Автор: {resource.Author}");
                Console.WriteLine($"Умови використання: {resource.UsageConditions}");
                Console.WriteLine($"Адреса: {resource.Address}");
                Console.WriteLine(new string('-', 30));
            }
        }
    }
}
