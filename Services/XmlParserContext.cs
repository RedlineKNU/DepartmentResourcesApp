public class XmlParserContext
{
    private IXmlParsingStrategy _strategy;

    // Конструктор, який встановлює початкову стратегію парсингу
    public XmlParserContext(IXmlParsingStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IXmlParsingStrategy strategy)
    {
        _strategy = strategy;
    }

    // Метод для аналізу XML-файлу з використанням поточної стратегії
    public string Analyze(string xmlFilePath)
    {
        return _strategy.Analyze(xmlFilePath);  
    }
}
