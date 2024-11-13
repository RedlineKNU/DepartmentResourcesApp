namespace DepartmentResourcesApp.Services
{
    public class XmlParserContext
    {
        private IXmlParsingStrategy _strategy;

        public void SetStrategy(IXmlParsingStrategy strategy)
        {
            _strategy = strategy;
        }

        public void ExecuteStrategy(string filePath)
        {
            _strategy?.Parse(filePath);
        }
    }
}