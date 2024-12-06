using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Credits.xaml
    /// </summary>
    public partial class Credits : Page
    {
        // Private Instanz des MainModel
        private MainModel? _pmodel;

        // Konstruktor der Credits-Klasse
        public Credits()
        {
            InitializeComponent();
            // Initialisiert _pmodel mit einer Ressource namens "pmodel" oder erstellt eine neue Instanz
            _pmodel = FindResource("pmodel") as MainModel ?? new();
        }

        // Event-Handler für das Navigieren eines Hyperlinks
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Öffnet den Hyperlink im Standardbrowser
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true; // Markiert das Ereignis als behandelt
        }

        // Event-Handler für den Klick auf einen Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Navigiert zur letzten Seite im Verlauf
            _pmodel?.Navigate(_pmodel.History[^1], false);
        }
    }


}
