//using System;
//using System.Collections.Generic;
//using System.Xml;
//using DepartmentResourcesApp.Services;


//namespace DepartmentResourcesApp.Services
//{
//    public class SAXParser : IXmlParsingStrategy
//    {
//        public void Parse(string filePath)
//        {
//            List<string> analysisResults = new List<string>
//        {
//            "Аналіз за допомогою SAX:",
//            new string('-', 30)
//        };

//            try
//            {
//                using (XmlReader reader = XmlReader.Create(filePath))
//                {
//                    while (reader.Read())
//                    {
//                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "resource")
//                        {
//                            if (reader.HasAttributes)
//                            {
//                                while (reader.MoveToNextAttribute())
//                                {
//                                    analysisResults.Add($"{reader.Name}: {reader.Value}");
//                                }
//                            }
//                        }

//                        if (reader.NodeType == XmlNodeType.Element && reader.Depth > 1)
//                        {
//                            string elementName = reader.Name;
//                            reader.Read();
//                            if (reader.NodeType == XmlNodeType.Text)
//                            {
//                                analysisResults.Add($"{elementName}: {reader.Value}");
//                            }
//                        }

//                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "resource")
//                        {
//                            analysisResults.Add(new string('-', 30));
//                        }
//                    }
//                }
//                Console.WriteLine(string.Join(Environment.NewLine, analysisResults));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Помилка під час аналізу SAX: " + ex.Message);
//            }
//        }
//    }
//}

    