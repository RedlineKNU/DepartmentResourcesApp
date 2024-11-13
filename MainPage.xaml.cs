using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace DepartmentResourcesApp
{
    public partial class MainPage : ContentPage
    {
        private string xmlFilePath;
        private string xslFilePath;
        private List<string> availableAttributes = new List<string>();

        public MainPage()
        {
            InitializeComponent();
        }

        // Завантаження файлу XML
        private async void OnFileLoadClicked(object sender, EventArgs e)
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть файл XML",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/xml" } },
                    { DevicePlatform.iOS, new[] { "public.xml" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.xml" } },
                    { DevicePlatform.WinUI, new[] { ".xml" } }
                })
            });

            if (file != null)
            {
                xmlFilePath = file.FullPath;
                filePathLabel.Text = Path.GetFileName(xmlFilePath);
                LoadAvailableAttributes();
            }
        }

        // Завантаження файлу XSL
        private async void OnXslFileLoadClicked(object sender, EventArgs e)
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть файл XSL",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/xml" } },
                    { DevicePlatform.iOS, new[] { "public.xml" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.xml" } },
                    { DevicePlatform.WinUI, new[] { ".xsl" } }
                })
            });

            if (file != null)
            {
                xslFilePath = file.FullPath;
                xslPathLabel.Text = Path.GetFileName(xslFilePath);
            }
        }

        // «підгрузка даних»
        private void LoadAvailableAttributes()
        {
            if (string.IsNullOrEmpty(xmlFilePath)) return;

            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                availableAttributes = doc.Descendants()
                                         .Attributes()
                                         .Select(a => a.Name.LocalName)
                                         .Distinct()
                                         .ToList();
                attributePicker.ItemsSource = availableAttributes;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Не вдалося завантажити атрибути: " + ex.Message, "OK");
            }
        }

        // Аналізування XML і відображення всіх данних в полі результатів
        private async void OnAnalyzeClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || strategyPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Виберіть XML-файл і метод обробки", "OK");
                return;
            }

            string method = strategyPicker.SelectedItem.ToString();
            string analysisResult = method switch
            {
                "SAX" => AnalyzeWithSAX(),
                "DOM" => AnalyzeWithDOM(),
                "LINQ" => AnalyzeWithLINQ(),
                _ => "Невідомий метод аналізу"
            };

            analysisOutputEditor.Text = analysisResult;
        }


        private string AnalyzeWithSAX()
        {
            try
            {
                List<string> analysisResults = new List<string>();

                analysisResults.Add("Аналіз за допомогою SAX:");
                analysisResults.Add(new string('-', 30));

                using (XmlReader reader = XmlReader.Create(xmlFilePath))
                {
                    while (reader.Read())
                    {
                        // Початок елемента "resource"
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "resource")
                        {
                            // Обробка атрибутів
                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    analysisResults.Add($"{reader.Name}: {reader.Value}");
                                }
                            }
                        }

                        // Обробка дочірніх елементів
                        if (reader.NodeType == XmlNodeType.Element && reader.Depth > 1)
                        {
                            string elementName = reader.Name;
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Text)
                            {
                                analysisResults.Add($"{elementName}: {reader.Value}");
                            }
                        }

                        // Закінчення елемента "resource"
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "resource")
                        {
                            analysisResults.Add(new string('-', 30));
                        }
                    }
                }

                return string.Join(Environment.NewLine, analysisResults);
            }
            catch (Exception ex)
            {
                return "Помилка під час аналізу SAX: " + ex.Message;
            }
        }



        private string AnalyzeWithDOM()
        {
            try
            {
                List<string> analysisResults = new List<string>
        {
            "Аналіз за допомогою DOM:",
            new string('-', 30)
        };

                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);

                XmlNodeList resources = doc.GetElementsByTagName("resource");

                foreach (XmlNode resource in resources)
                {
                    if (resource.Attributes != null)
                    {
                        foreach (XmlAttribute attr in resource.Attributes)
                        {
                            analysisResults.Add($"{attr.Name}: {attr.Value}");
                        }
                    }

                    foreach (XmlNode child in resource.ChildNodes)
                    {
                        analysisResults.Add($"{child.Name}: {child.InnerText}");
                    }

                    analysisResults.Add(new string('-', 30));
                }

                return string.Join(Environment.NewLine, analysisResults);
            }
            catch (Exception ex)
            {
                return "Помилка під час аналізу DOM: " + ex.Message;
            }
        }


        private string AnalyzeWithLINQ()
        {
            try
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
            catch (Exception ex)
            {
                return "Помилка під час аналізу LINQ: " + ex.Message;
            }
        }




        // Перетворення XML на HTML
        private async void OnTransformClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || string.IsNullOrEmpty(xslFilePath))
            {
                await DisplayAlert("Error", "Виберіть файли XML і XSL", "OK");
                return;
            }

            try
            {
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xslFilePath);

                string htmlOutputPath = Path.Combine(FileSystem.CacheDirectory, "output.html");
                using (var xmlReader = XmlReader.Create(xmlFilePath))
                using (var writer = XmlWriter.Create(htmlOutputPath))
                {
                    xslt.Transform(xmlReader, writer);
                }

                htmlWebView.Source = htmlOutputPath;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Не вдалося перетворити XML на HTML: " + ex.Message, "OK");
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




        // Очистка усіх полів 
        private void OnClearClicked(object sender, EventArgs e)
        {
            filePathLabel.Text = "Файл не вибрано";
            xslPathLabel.Text = "Файл не вибрано";
            xmlFilePath = null;
            xslFilePath = null;
            attributePicker.SelectedIndex = -1;
            attributeValueEntry.Text = string.Empty;
            strategyPicker.SelectedIndex = -1;
            analysisOutputEditor.Text = string.Empty;
            htmlWebView.Source = new HtmlWebViewSource { Html = string.Empty }; //Очистка поля для відображення HTML
        }

        // Вихід з програми
        private async void OnExitClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Вихід", "Ви впевнені, що хочете вийти?", "Так", "Ні");
            if (confirm)
            {
                Application.Current.Quit();
            }
        }
    }
}



