using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;
using DepartmentResourcesApp.Services;

namespace DepartmentResourcesApp
{
    public partial class MainPage : ContentPage
    {
        private XmlParserContext _context = new XmlParserContext();
        private string _filePath;

        // Власний тип для вибору XML файлів
        private static readonly FilePickerFileType XmlFileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xml" } },
                { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
            });

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnFileLoadClicked(object sender, EventArgs e)
        {
            // Завантаження XML файлу
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть XML файл",
                FileTypes = XmlFileType
            });

            if (result != null)
            {
                _filePath = result.FullPath;
                filePathLabel.Text = $"Файл: {Path.GetFileName(_filePath)} завантажено";
            }
            else
            {
                await DisplayAlert("Помилка", "Файл не було вибрано", "OK");
            }
        }

        private void OnAnalyzeClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                DisplayAlert("Помилка", "Будь ласка, завантажте XML файл", "OK");
                return;
            }

            string selectedStrategy = (string)strategyPicker.SelectedItem;

            // Вибір стратегії
            switch (selectedStrategy)
            {
                case "SAX":
                    _context.SetStrategy(new SAXParser());
                    break;
                case "DOM":
                    _context.SetStrategy(new DOMParser());
                    break;
                case "LINQ":
                    _context.SetStrategy(new LINQParser());
                    break;
                default:
                    DisplayAlert("Помилка", "Виберіть спосіб обробки", "OK");
                    return;
            }

            _context.ExecuteStrategy(_filePath);
        }

        private void OnTransformClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                DisplayAlert("Помилка", "Будь ласка, завантажте XML файл", "OK");
                return;
            }

            string xslFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "transform.xsl");
            string htmlOutputPath = Path.Combine(Environment.CurrentDirectory, "Data", "output.html");

            TransformXmlToHtml(_filePath, xslFilePath, htmlOutputPath);
            DisplayAlert("Інформація", "Трансформація у HTML виконана", "OK");
        }

        private void TransformXmlToHtml(string xmlPath, string xslPath, string outputHtmlPath)
        {
            try
            {
                var xslt = new System.Xml.Xsl.XslCompiledTransform();
                xslt.Load(xslPath);
                xslt.Transform(xmlPath, outputHtmlPath);

                Console.WriteLine($"HTML файл збережено в: {outputHtmlPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час трансформації: {ex.Message}");
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            // Очищення вибору та полів
            filePathLabel.Text = "Файл не вибрано";
            _filePath = string.Empty;
            strategyPicker.SelectedIndex = -1;
        }
    }
}
