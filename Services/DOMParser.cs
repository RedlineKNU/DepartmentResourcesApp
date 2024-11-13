using System.Xml;

public class DOMParser : IXmlParsingStrategy
{
    public string Analyze(string xmlFilePath)
    {
        List<string> analysisResults = new List<string>
        {
            "Аналіз за допомогою DOM:",
            new string('-', 30)
        };

        XmlDocument doc = new XmlDocument();
        doc.Load(xmlFilePath);

        XmlNodeList resources = doc.GetElementsByTagName("resource");

        foreach (XmlNode resource in resources)
        {
            if (resource.Attributes != null)
            {
                foreach (XmlAttribute attr in resource.Attributes)
                {
                    analysisResults.Add($"{attr.Name}: {attr.Value}");
                }
            }

            foreach (XmlNode child in resource.ChildNodes)
            {
                analysisResults.Add($"{child.Name}: {child.InnerText}");
            }

            analysisResults.Add(new string('-', 30));
        }

        return string.Join(Environment.NewLine, analysisResults);
    }
}
