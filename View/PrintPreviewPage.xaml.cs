using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    public partial class PrintPreviewPage : Page
    {
        private MainModel? _model;

        public PrintPreviewPage(CoreWebView2 coreWebView2)
        {
            InitializeComponent();
            _model = FindResource("pmodel") as MainModel;
            InitializeWebView(coreWebView2);
            MaximizeWindow();
        }

        private async void InitializeWebView(CoreWebView2 coreWebView2)
        {
            await webViewPreview.EnsureCoreWebView2Async();

            this.webViewPreview.CoreWebView2.Settings.HiddenPdfToolbarItems =
                CoreWebView2PdfToolbarItems.Save |
                CoreWebView2PdfToolbarItems.SaveAs |
                CoreWebView2PdfToolbarItems.MoreSettings |
                CoreWebView2PdfToolbarItems.Print |
                CoreWebView2PdfToolbarItems.FullScreen;

            webViewPreview.CoreWebView2.NavigationCompleted += async (s, e) =>
            {
                if (e.IsSuccess)
                {
                    Console.WriteLine("Navigation abgeschlossen. Warten auf Rendern...");
                    await Task.Delay(500); // Kurze Verzögerung, um sicherzustellen, dass der Inhalt gerendert wird.
                    try
                    {
                        webViewPreview.CoreWebView2.ShowPrintUI();
                    }
                    catch (Exception ex)
                    {
                        new Popup().Display("Fehler", $"Fehler beim Öffnen der Druckvorschau: {ex.Message}", PopupType.Error, PopupButtons.Ok);
                    }
                }
                else
                {
                    Console.WriteLine($"Navigation fehlgeschlagen: {e.WebErrorStatus}");
                }
            };

            webViewPreview.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;

            webViewPreview.CoreWebView2.Navigate(coreWebView2.Source);
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object sender, object e)
        {
            if (webViewPreview.CoreWebView2.ContainsFullScreenElement)
            {
                webViewPreview.CoreWebView2.ExecuteScriptAsync("document.exitFullscreen();");
                MessageBox.Show("Vollbildmodus ist deaktiviert.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MaximizeWindow()
        {
            Window mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeWindow()
        {
            Window mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
        }

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                webViewPreview.CoreWebView2.ShowPrintUI();
            }
            catch (Exception ex)
            {
                new Popup().Display("Fehler", $"Fehler beim Drucken der PDF: {ex.Message}", PopupType.Error, PopupButtons.Ok);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            webViewPreview.CoreWebView2.ShowPrintUI();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MinimizeWindow();
            _model?.Navigate(_model.History[^1], false);
        }
    }
}
