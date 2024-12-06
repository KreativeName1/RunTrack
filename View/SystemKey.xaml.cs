using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Key.xaml
    /// </summary>
    public partial class SystemKey : Page
    {
        // Konstruktor der Klasse SystemKey
        public SystemKey()
        {
            InitializeComponent(); // Initialisiert die Komponenten der Seite
            tbKey.Text = UniqueKey.GetKey(); // Setzt den Text des TextBlocks tbKey auf einen eindeutigen Schlüssel
        }

        // Event-Handler für den Klick auf den Button "Schließen"
        private void btnSchliessen_Click(object sender, RoutedEventArgs e)
        {
            // Holt das MainModel aus den Ressourcen
            MainModel model = FindResource("pmodel") as MainModel;

            // Navigiert zur letzten Seite in der Verlaufsliste, falls das Modell nicht null ist
            model?.Navigate(model.History[^1], false);
        }
    }
}
