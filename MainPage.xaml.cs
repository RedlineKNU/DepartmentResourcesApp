using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            // Removed PopulateAttributePicker(); since LoadAvailableAttributes is handling attribute loading
        }

        // Load XML File
        private async void OnFileLoadClicked(object sender, EventArgs e)
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select XML File",
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

        // Load XSL File
        private async void OnXslFileLoadClicked(object sender, EventArgs e)
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select XSL File",
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

        // Load Attributes into the Picker from XML
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
                DisplayAlert("Error", "Failed to load attributes: " + ex.Message, "OK");
            }
        }

        // Analyze XML
        private async void OnAnalyzeClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || strategyPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Please select XML file and processing method", "OK");
                return;
            }

            string method = strategyPicker.SelectedItem.ToString();
            string analysisResult = method switch
            {
                "SAX" => AnalyzeWithSAX(),
                "DOM" => AnalyzeWithDOM(),
                "LINQ" => AnalyzeWithLINQ(),
                _ => "Unknown analysis method"
            };

            analysisOutputEditor.Text = analysisResult;
        }

        private string AnalyzeWithSAX()
        {
            // Implement SAX-based XML analysis logic here
            return "SAX analysis not yet implemented.";
        }

        private string AnalyzeWithDOM()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);
                return "DOM analysis successful.";
            }
            catch (Exception ex)
            {
                return "Error during DOM analysis: " + ex.Message;
            }
        }

        private string AnalyzeWithLINQ()
        {
            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                return "LINQ analysis successful.";
            }
            catch (Exception ex)
            {
                return "Error during LINQ analysis: " + ex.Message;
            }
        }

        // Transform XML to HTML
        private async void OnTransformClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || string.IsNullOrEmpty(xslFilePath))
            {
                await DisplayAlert("Error", "Please select both XML and XSL files", "OK");
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
                await DisplayAlert("Error", "Failed to transform XML to HTML: " + ex.Message, "OK");
            }
        }

        // Apply Filter based on selected attribute and value
        private async void OnFilterApplyClicked(object sender, EventArgs e)
        {
            if (attributePicker.SelectedIndex == -1 || string.IsNullOrEmpty(attributeValueEntry.Text))
            {
                await DisplayAlert("Error", "Please select an attribute and enter a value to filter", "OK");
                return;
            }

            string selectedAttribute = attributePicker.SelectedItem.ToString();
            string attributeValue = attributeValueEntry.Text;

            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                var filteredElements = doc.Descendants()
                                          .Where(el => el.Attribute(selectedAttribute)?.Value == attributeValue);

                if (filteredElements.Any())
                {
                    analysisOutputEditor.Text = string.Join(Environment.NewLine, filteredElements.Select(el => el.ToString()));
                }
                else
                {
                    analysisOutputEditor.Text = "No elements found with the specified attribute and value.";
                }
            }
            catch (Exception ex)
            {
                analysisOutputEditor.Text = "Error during filtering: " + ex.Message;
            }
        }

        // Clear all fields
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
            htmlWebView.Source = null;
        }

        // Exit application
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
