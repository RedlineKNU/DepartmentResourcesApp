using System;
using System.Xml;

namespace DepartmentResourcesApp.Services
{
    public class DOMParser : IXmlParsingStrategy
    {
        public void Parse(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList resources = doc.GetElementsByTagName("resource");

            foreach (XmlNode resource in resources)
            {
                var title = resource.Attributes["title"]?.InnerText;
                var type = resource.Attributes["type"]?.InnerText;

                Console.WriteLine($"Назва: {title}");
                Console.WriteLine($"Тип: {type}");

                foreach (XmlNode child in resource.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "annotation":
                            Console.WriteLine($"Анотація: {child.InnerText}");
                            break;
                        case "author":
                            Console.WriteLine($"Автор: {child.InnerText}");
                            break;
                        case "usage_conditions":
                            Console.WriteLine($"Умови використання: {child.InnerText}");
                            break;
                        case "address":
                            Console.WriteLine($"Адреса: {child.InnerText}");
                            break;
                    }
                }
                Console.WriteLine(new string('-', 30));
            }
        }
    }
}
