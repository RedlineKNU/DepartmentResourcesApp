using System;
using System.Xml;

namespace DepartmentResourcesApp.Services
{
    public class SAXParser : IXmlParsingStrategy
    {
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

                        Console.WriteLine($"Назва: {title}");
                        Console.WriteLine($"Тип: {type}");
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "annotation")
                    {
                        Console.WriteLine($"Анотація: {reader.ReadElementContentAsString()}");
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "author")
                    {
                        Console.WriteLine($"Автор: {reader.ReadElementContentAsString()}");
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "usage_conditions")
                    {
                        Console.WriteLine($"Умови використання: {reader.ReadElementContentAsString()}");
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "address")
                    {
                        Console.WriteLine($"Адреса: {reader.ReadElementContentAsString()}");
                    }

                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "resource")
                    {
                        Console.WriteLine(new string('-', 30));
                    }
                }
            }
        }
    }
}
