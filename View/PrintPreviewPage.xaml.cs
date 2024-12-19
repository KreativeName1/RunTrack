using Microsoft.Web.WebView2.Core;
using System;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für PrintPreviewPage.xaml
    /// </summary>
    public partial class PrintPreviewPage : Page
    {
        public PrintPreviewPage(CoreWebView2 coreWebView2)
        {
            InitializeComponent();
            InitializeWebView(coreWebView2);
        }

        private async void InitializeWebView(CoreWebView2 coreWebView2)
        {
            await webViewPreview.EnsureCoreWebView2Async();

            webViewPreview.CoreWebView2.DOMContentLoaded += (s, e) =>
            {
                Console.WriteLine("DOM vollständig geladen.");
                try
                {
                    webViewPreview.CoreWebView2.ShowPrintUI();
                }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler", $"Fehler beim Öffnen der Druckvorschau: {ex.Message}", PopupType.Error, PopupButtons.Ok);
                }
            };

            webViewPreview.CoreWebView2.Navigate(coreWebView2.Source);
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
    }
}
