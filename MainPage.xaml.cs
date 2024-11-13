using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DepartmentResourcesApp
{
    public partial class MainPage : ContentPage
    {
        private string xmlFilePath;
        private string xslFilePath;
        private List<string> availableAttributes = new List<string>();
        private XmlParserContext _parserContext;

        public MainPage()
        {
            InitializeComponent();
            _parserContext = new XmlParserContext(new SAXParser()); // Початкова стратегія (SAX)
        }

        private async void OnFileLoadClicked(object sender, EventArgs e)
        {
            // Завантаження XML-файлу
            var xmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xml" } },
                { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть XML файл",
                FileTypes = xmlFileType
            });

            if (result != null)
            {
                xmlFilePath = result.FullPath;
                filePathLabel.Text = $"Вибрано файл: {Path.GetFileName(xmlFilePath)}";

                // Оновити доступні атрибути
                UpdateAvailableAttributes();
            }
        }

        private async void OnXslFileLoadClicked(object sender, EventArgs e)
        {
            // Завантаження XSL-файлу
            var xslFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xsl" } },
                { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть XSL файл",
                FileTypes = xslFileType
            });

            if (result != null)
            {
                xslFilePath = result.FullPath;
                xslPathLabel.Text = $"Вибрано файл: {Path.GetFileName(xslFilePath)}";
            }
        }

        private async void OnAnalyzeClicked(object sender, EventArgs e)
        {
            // Перевірка на вибір файлу та методу
            if (string.IsNullOrEmpty(xmlFilePath) || strategyPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Помилка", "Виберіть XML-файл і метод обробки", "OK");
                return;
            }

            // Вибір стратегії на основі обраного методу
            IXmlParsingStrategy strategy = strategyPicker.SelectedItem.ToString() switch
            {
                "SAX" => new SAXParser(),
                "DOM" => new DOMParser(),
                "LINQ" => new LINQParser(),
                _ => null
            };

            if (strategy == null) return;

            // Встановити обрану стратегію в контексті та виконати аналіз
            _parserContext.SetStrategy(strategy);
            string analysisResult = _parserContext.Analyze(xmlFilePath);
            analysisOutputEditor.Text = analysisResult;
        }

        private async void OnTransformClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || string.IsNullOrEmpty(xslFilePath))
            {
                await DisplayAlert("Помилка", "Виберіть XML та XSL файли", "OK");
                return;
            }

            // Виконання трансформації XML -> HTML
            string htmlContent = XslTransformer.Transform(xmlFilePath, xslFilePath);
            if (!string.IsNullOrEmpty(htmlContent))
            {
                htmlWebView.Source = new HtmlWebViewSource { Html = htmlContent };
            }
        }



        private void OnClearClicked(object sender, EventArgs e)
        {
            // Очищення полів виводу та вибору параметрів
            analysisOutputEditor.Text = string.Empty;
            htmlWebView.Source = new HtmlWebViewSource { Html = string.Empty }; //Очистка поля для відображення HTML
            attributePicker.SelectedIndex = -1;
            attributeValueEntry.Text = string.Empty;
            filePathLabel.Text = "Файл не вибрано";
            xslPathLabel.Text = "Файл не вибрано";
            xmlFilePath = null;
            xslFilePath = null;
        }

        private async void OnExitClicked(object sender, EventArgs e)
        {
            bool confirmExit = await DisplayAlert("Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
            if (confirmExit)
            {
                Application.Current.Quit();
            }
        }

        private void UpdateAvailableAttributes()
        {
            // Оновлення списку доступних атрибутів для вибору в `attributePicker`
            if (!string.IsNullOrEmpty(xmlFilePath))
            {
                availableAttributes = XmlAttributeExtractor.ExtractAttributes(xmlFilePath, "resource");
                attributePicker.ItemsSource = availableAttributes;
            }
        }

        // Застосування фільтру на основі вибраного атрибута та значення
        private async void OnFilterApplyClicked(object sender, EventArgs e)
        {
            if (attributePicker.SelectedIndex == -1 || string.IsNullOrEmpty(attributeValueEntry.Text))
            {
                await DisplayAlert("Error", "Виберіть атрибут і введіть значення для фільтрування", "OK");
                return;
            }

            string selectedAttribute = attributePicker.SelectedItem.ToString();
            string attributeValue = attributeValueEntry.Text;

            // Опрацювання операторів для років (<, <=, >, >=)
            string operatorSymbol = string.Empty;
            if (attributeValue.StartsWith("<="))
            {
                operatorSymbol = "<=";
                attributeValue = attributeValue.Substring(2).Trim();
            }
            else if (attributeValue.StartsWith("<"))
            {
                operatorSymbol = "<";
                attributeValue = attributeValue.Substring(1).Trim();
            }
            else if (attributeValue.StartsWith(">="))
            {
                operatorSymbol = ">=";
                attributeValue = attributeValue.Substring(2).Trim();
            }
            else if (attributeValue.StartsWith(">"))
            {
                operatorSymbol = ">";
                attributeValue = attributeValue.Substring(1).Trim();
            }

            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                var filteredElements = doc.Descendants("resource")
                                          .Where(el =>
                                          {
                                              var attribute = el.Attribute(selectedAttribute)?.Value;
                                              if (attribute != null)
                                              {
                                                  // Спроба парснути значення атрибута як число для порівняння
                                                  if (double.TryParse(attribute, out double attributeNumber) && double.TryParse(attributeValue, out double filterNumber))
                                                  {
                                                      switch (operatorSymbol)
                                                      {
                                                          case "<=":
                                                              return attributeNumber <= filterNumber;
                                                          case "<":
                                                              return attributeNumber < filterNumber;
                                                          case ">=":
                                                              return attributeNumber >= filterNumber;
                                                          case ">":
                                                              return attributeNumber > filterNumber;
                                                          default:
                                                              return attribute == attributeValue; // немає оператору, тому ідеально підходе
                                                      }
                                                  }
                                                  else
                                                  {
                                                      // Якщо атрибут не є числом, виконується порівняння рядків
                                                      switch (operatorSymbol)
                                                      {
                                                          case "<=":
                                                              return string.Compare(attribute, attributeValue) <= 0;
                                                          case "<":
                                                              return string.Compare(attribute, attributeValue) < 0;
                                                          case ">=":
                                                              return string.Compare(attribute, attributeValue) >= 0;
                                                          case ">":
                                                              return string.Compare(attribute, attributeValue) > 0;
                                                          default:
                                                              return attribute.Contains(attributeValue, StringComparison.OrdinalIgnoreCase); // default to partial match
                                                      }
                                                  }
                                              }
                                              return false;
                                          });

                if (filteredElements.Any())
                {
                    analysisOutputEditor.Text = string.Empty;
                    foreach (var element in filteredElements)
                    {
                        foreach (var attr in element.Attributes())
                        {
                            analysisOutputEditor.Text += $"{attr.Name}: {attr.Value}\n";
                        }

                        foreach (var child in element.Elements())
                        {
                            analysisOutputEditor.Text += $"{child.Name}: {child.Value}\n";
                        }

                        analysisOutputEditor.Text += new string('-', 30) + "\n";
                    }
                }
                else
                {
                    analysisOutputEditor.Text = "Не знайдено елементів із зазначеним атрибутом і значенням.";
                }
            }
            catch (Exception ex)
            {
                analysisOutputEditor.Text = "Помилка під час фільтрації: " + ex.Message;
            }
        }

    }

    public static class XmlAttributeExtractor
    {
        public static List<string> ExtractAttributes(string xmlFilePath, string nodeName)
        {
            var attributes = new List<string>();

            // Парсинг XML для знаходження унікальних атрибутів
            var doc = new System.Xml.XmlDocument();
            doc.Load(xmlFilePath);
            var nodes = doc.GetElementsByTagName(nodeName);

            foreach (System.Xml.XmlNode node in nodes)
            {
                foreach (System.Xml.XmlAttribute attribute in node.Attributes)
                {
                    if (!attributes.Contains(attribute.Name))
                    {
                        attributes.Add(attribute.Name);
                    }
                }
            }

            return attributes;
        }
    }

    public static class XslTransformer
    {
        public static string Transform(string xmlFilePath, string xslFilePath)
        {
            var xslTransform = new System.Xml.Xsl.XslCompiledTransform();
            xslTransform.Load(xslFilePath);

            using var xmlReader = System.Xml.XmlReader.Create(xmlFilePath);
            using var stringWriter = new StringWriter();
            using var xmlWriter = System.Xml.XmlWriter.Create(stringWriter);

            xslTransform.Transform(xmlReader, xmlWriter);
            return stringWriter.ToString();
        }
    }
}
