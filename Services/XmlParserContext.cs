using System;
using DepartmentResourcesApp.Services;

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
            if (_strategy != null)
            {
                _strategy.Parse(filePath);
            }
            else
            {
                Console.WriteLine("Стратегія обробки XML не встановлена.");
            }
        }
    }
}
