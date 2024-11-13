
//using System.Xml.Linq;
//using DepartmentResourcesApp.Services;


//namespace DepartmentResourcesApp.Services
//{
//    public class LINQParser : IXmlParsingStrategy
//    {
//        public void Parse(string filePath)
//        {
//            List<string> analysisResults = new List<string>
//        {
//            "Аналіз за допомогою LINQ to XML:",
//            new string('-', 30)
//        };

//            try
//            {
//                XDocument doc = XDocument.Load(filePath);
//                var resources = doc.Descendants("resource");

//                foreach (var resource in resources)
//                {
//                    foreach (var attr in resource.Attributes())
//                    {
//                        analysisResults.Add($"{attr.Name}: {attr.Value}");
//                    }

//                    foreach (var element in resource.Elements())
//                    {
//                        analysisResults.Add($"{element.Name}: {element.Value}");
//                    }
//                    analysisResults.Add(new string('-', 30));
//                }

//                Console.WriteLine(string.Join(Environment.NewLine, analysisResults));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Помилка під час аналізу LINQ: " + ex.Message);
//            }
//        }
//    }
//}