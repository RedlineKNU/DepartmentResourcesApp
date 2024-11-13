using System.Xml;

public class SAXParser : IXmlParsingStrategy
{
    public string Analyze(string xmlFilePath)
    {
        List<string> analysisResults = new List<string>
        {
            "Аналіз за допомогою SAX:",
            new string('-', 30)
        };

        using (XmlReader reader = XmlReader.Create(xmlFilePath))
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "resource")
                {
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            analysisResults.Add($"{reader.Name}: {reader.Value}");
                        }
                    }
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Depth > 1)
                {
                    string elementName = reader.Name;
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        analysisResults.Add($"{elementName}: {reader.Value}");
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "resource")
                {
                    analysisResults.Add(new string('-', 30));
                }
            }
        }

        return string.Join(Environment.NewLine, analysisResults);
    }
}
