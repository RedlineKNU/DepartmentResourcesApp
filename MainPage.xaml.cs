using System.Xml.Linq;

namespace DepartmentResourcesApp
{
    public partial class MainPage : ContentPage
    {
        // Змінні для зберігання шляхів до файлів
        private string xmlFilePath;
        private string xslFilePath;

        // Список для зберігання доступних атрибутів XML-файлу
        private List<string> availableAttributes = new List<string>();

        // Контекст для вибору стратегії парсингу XML
        private XmlParserContext _parserContext;

        public MainPage()
        {
            InitializeComponent();
            // Ініціалізація початкової стратегії парсингу (SAX)
            _parserContext = new XmlParserContext(new SAXParser());
        }

        private async void OnFileLoadClicked(object sender, EventArgs e)
        {
            var xmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xml" } },
                { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
            });

            // Відкриваємо діалог вибору файлу
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть XML файл", 
                FileTypes = xmlFileType
            });

            if (result != null)
            {
                xmlFilePath = result.FullPath; 
                filePathLabel.Text = $"Вибрано файл: {Path.GetFileName(xmlFilePath)}"; 

                // Оновлюємо доступні атрибути для вибору
                UpdateAvailableAttributes();
            }
        }

        private async void OnXslFileLoadClicked(object sender, EventArgs e)
        {
            var xslFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xsl" } },
                { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
            });

            // Відкриваємо діалог вибору XSL файлу
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
            // Перевірка, чи вибрано XML-файл та метод парсингу
            if (string.IsNullOrEmpty(xmlFilePath) || strategyPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Помилка", "Виберіть XML-файл і метод обробки", "OK");
                return;
            }

            // Вибір стратегії парсингу XML залежно від обраного методу
            IXmlParsingStrategy strategy = strategyPicker.SelectedItem.ToString() switch
            {
                "SAX" => new SAXParser(), 
                "DOM" => new DOMParser(), 
                "LINQ" => new LINQParser(), 
                _ => null
            };

            if (strategy == null) return;

            // Встановлюємо обрану стратегію та виконуємо аналіз
            _parserContext.SetStrategy(strategy);
            string analysisResult = _parserContext.Analyze(xmlFilePath); 
            analysisOutputEditor.Text = analysisResult; 
        }

        // Обробка натискання на кнопку для виконання трансформації XML -> HTML
        private async void OnTransformClicked(object sender, EventArgs e)
        {
            // Перевірка, чи вибрано XML та XSL файли
            if (string.IsNullOrEmpty(xmlFilePath) || string.IsNullOrEmpty(xslFilePath))
            {
                await DisplayAlert("Помилка", "Виберіть XML та XSL файли", "OK");
                return;
            }

            // Виконання трансформації XML в HTML за допомогою XSL
            string htmlContent = XslTransformer.Transform(xmlFilePath, xslFilePath);
            if (!string.IsNullOrEmpty(htmlContent))
            {
                htmlWebView.Source = new HtmlWebViewSource { Html = htmlContent }; 
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            analysisOutputEditor.Text = string.Empty;
            htmlWebView.Source = new HtmlWebViewSource { Html = string.Empty }; // Очистка HTML-переглядача
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
            if (!string.IsNullOrEmpty(xmlFilePath))
            {
                availableAttributes = XmlAttributeExtractor.ExtractAttributes(xmlFilePath, "resource"); // Отримуємо атрибути для ресурсу
                attributePicker.ItemsSource = availableAttributes; // Оновлюємо джерело елементів для Picker
            }
        }

        private async void OnFilterApplyClicked(object sender, EventArgs e)
        {
            // Перевірка, чи вибрано атрибут та введено значення для фільтру
            if (attributePicker.SelectedIndex == -1 || string.IsNullOrEmpty(attributeValueEntry.Text))
            {
                await DisplayAlert("Помилка", "Виберіть атрибут і введіть значення для фільтрування", "OK");
                return;
            }

            // Отримуємо вибраний атрибут та значення для фільтрування
            string selectedAttribute = attributePicker.SelectedItem.ToString();
            string attributeValue = attributeValueEntry.Text;

            // Обробка операторів порівняння для чисел та рядків
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
                                                  // Парсинг значень атрибутів для порівняння
                                                  if (double.TryParse(attribute, out double attributeNumber) && double.TryParse(attributeValue, out double filterNumber))
                                                  {
                                                      switch (operatorSymbol)
                                                      {
                                                          case "<=": return attributeNumber <= filterNumber;
                                                          case "<": return attributeNumber < filterNumber;
                                                          case ">=": return attributeNumber >= filterNumber;
                                                          case ">": return attributeNumber > filterNumber;
                                                          default: return attribute == attributeValue;
                                                      }
                                                  }
                                                  else
                                                  {
                                                      switch (operatorSymbol)
                                                      {
                                                          case "<=": return string.Compare(attribute, attributeValue) <= 0;
                                                          case "<": return string.Compare(attribute, attributeValue) < 0;
                                                          case ">=": return string.Compare(attribute, attributeValue) >= 0;
                                                          case ">": return string.Compare(attribute, attributeValue) > 0;
                                                          default: return attribute.Contains(attributeValue, StringComparison.OrdinalIgnoreCase);
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
        // Статичний метод для витягування унікальних атрибутів з елементів XML за заданим тегом
        public static List<string> ExtractAttributes(string xmlFilePath, string nodeName)
        {
            var attributes = new List<string>();
            var doc = new System.Xml.XmlDocument();
            doc.Load(xmlFilePath);

            // Отримуємо всі елементи з вказаним тегом (nodeName)
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
        // Статичний метод для трансформації XML файлу в HTML за допомогою XSLT
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

