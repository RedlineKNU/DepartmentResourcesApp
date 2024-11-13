using System.Xml;

public class DOMParser : IXmlParsingStrategy
{
    public string Analyze(string xmlFilePath)
    {
        List<string> analysisResults = new List<string>
        {
            "Аналіз за допомогою DOM:",  
            new string('-', 30)  // Роздільник для виводу
        };

        XmlDocument doc = new XmlDocument();
        doc.Load(xmlFilePath);  // Завантаження XML-файлу

        XmlNodeList resources = doc.GetElementsByTagName("resource");  

        foreach (XmlNode resource in resources)
        {
            if (resource.Attributes != null)
            {
                // Перебір атрибутів елемента "resource"
                foreach (XmlAttribute attr in resource.Attributes)
                {
                    analysisResults.Add($"{attr.Name}: {attr.Value}");  
                }
            }

            foreach (XmlNode child in resource.ChildNodes)
            {
                // Перебір дочірніх елементів "resource" і додавання їх до результатів
                analysisResults.Add($"{child.Name}: {child.InnerText}");
            }

            analysisResults.Add(new string('-', 30));  
        }

        return string.Join(Environment.NewLine, analysisResults);
    }
}
