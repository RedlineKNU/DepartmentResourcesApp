using DepartmentResourcesApp.Services;
using System.Xml.Xsl;
using System.IO;
using Microsoft.Maui.Controls;

namespace DepartmentResourcesApp
{
    public partial class MainPage : ContentPage
    {
        private XmlParserContext _context = new XmlParserContext();
        private string _filePath;
        private string _xslFilePath;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnFileLoadClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть XML файл",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
                {
                    { DevicePlatform.Android, new[] { ".xml" } },
                    { DevicePlatform.iOS, new[] { ".xml" } },
                    { DevicePlatform.WinUI, new[] { ".xml" } },
                    { DevicePlatform.macOS, new[] { ".xml" } }
                })
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

        private async void OnXslFileLoadClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть XSL файл",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
                {
                    { DevicePlatform.Android, new[] { ".xsl" } },
                    { DevicePlatform.iOS, new[] { ".xsl" } },
                    { DevicePlatform.WinUI, new[] { ".xsl" } },
                    { DevicePlatform.macOS, new[] { ".xsl" } }
                })
            });

            if (result != null)
            {
                _xslFilePath = result.FullPath;
                xslPathLabel.Text = $"Файл: {Path.GetFileName(_xslFilePath)} завантажено";
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
            analysisOutputEditor.Text = string.Empty;
            htmlWebView.Source = null; // Clear HTML view upon new analysis

            switch (selectedStrategy)
            {
                case "SAX":
                    _context.SetStrategy(new SAXParser(AppendAnalysisOutput));
                    break;
                case "DOM":
                    _context.SetStrategy(new DOMParser(AppendAnalysisOutput));
                    break;
                case "LINQ":
                    _context.SetStrategy(new LINQParser(AppendAnalysisOutput));
                    break;
                default:
                    DisplayAlert("Помилка", "Виберіть спосіб обробки", "OK");
                    return;
            }

            _context.ExecuteStrategy(_filePath);
        }

        private async void OnTransformClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath) || string.IsNullOrEmpty(_xslFilePath))
            {
                await DisplayAlert("Помилка", "Будь ласка, завантажте XML і XSL файли", "OK");
                return;
            }

            TransformXmlToHtml(_filePath, _xslFilePath);
        }

        private void TransformXmlToHtml(string xmlPath, string xslPath)
        {
            try
            {
                var xslt = new XslCompiledTransform();
                xslt.Load(xslPath);

                using (var writer = new StringWriter())
                {
                    xslt.Transform(xmlPath, null, writer);
                    // Set HTML content in WebView
                    htmlWebView.Source = new HtmlWebViewSource
                    {
                        Html = writer.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                analysisOutputEditor.Text = $"Помилка під час трансформації: {ex.Message}";
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            filePathLabel.Text = "Файл не вибрано";
            xslPathLabel.Text = "Файл не вибрано";
            _filePath = string.Empty;
            _xslFilePath = string.Empty;
            strategyPicker.SelectedIndex = -1;
            analysisOutputEditor.Text = string.Empty;
            htmlWebView.Source = new HtmlWebViewSource { Html = string.Empty };
        }

        private void AppendAnalysisOutput(string text)
        {
            analysisOutputEditor.Text += text + Environment.NewLine;
        }

        // Підтвердження при виході з програми
        private async void OnExitClicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Підтвердження", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
            if (result)
            {
                // Закрити додаток
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
