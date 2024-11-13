//using System;
//using System.Xml;



//namespace DepartmentResourcesApp.Services
//{
//    public class DOMParser : IXmlParsingStrategy
//    {
//        public void Parse(string filePath)
//        {
//            List<string> analysisResults = new List<string>
//        {
//            "Аналіз за допомогою DOM:",
//            new string('-', 30)
//        };

//            try
//            {
//                XmlDocument doc = new XmlDocument();
//                doc.Load(filePath);

//                XmlNodeList nodes = doc.GetElementsByTagName("resource");
//                foreach (XmlNode node in nodes)
//                {
//                    if (node.Attributes != null)
//                    {
//                        foreach (XmlAttribute attr in node.Attributes)
//                        {
//                            analysisResults.Add($"{attr.Name}: {attr.Value}");
//                        }
//                    }

//                    foreach (XmlNode childNode in node.ChildNodes)
//                    {
//                        analysisResults.Add($"{childNode.Name}: {childNode.InnerText}");
//                    }
//                    analysisResults.Add(new string('-', 30));
//                }

//                Console.WriteLine(string.Join(Environment.NewLine, analysisResults));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Помилка під час аналізу DOM: " + ex.Message);
//            }
//        }
//    }
//}