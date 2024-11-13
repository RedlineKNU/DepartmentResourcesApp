using System.Xml.Linq;

public class LINQParser : IXmlParsingStrategy
{
    public string Analyze(string xmlFilePath)
    {
        List<string> analysisResults = new List<string>
        {
            "Аналіз за допомогою LINQ:",
            new string('-', 30)
        };

        XDocument doc = XDocument.Load(xmlFilePath);
        var resources = doc.Descendants("resource");

        foreach (var resource in resources)
        {
            foreach (var attr in resource.Attributes())
            {
                analysisResults.Add($"{attr.Name}: {attr.Value}");
            }

            foreach (var element in resource.Elements())
            {
                analysisResults.Add($"{element.Name}: {element.Value}");
            }

            analysisResults.Add(new string('-', 30));
        }

        return string.Join(Environment.NewLine, analysisResults);
    }
}
