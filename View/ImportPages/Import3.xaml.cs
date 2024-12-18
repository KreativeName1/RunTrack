using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Import3.xaml
    /// </summary>
    public partial class Import3 : Page
    {
        private ImportModel _imodel; // ImportModel-Instanz
        private MainModel _model; // MainModel-Instanz

        // Konstruktor der Import3-Klasse
        public Import3(string meldung, bool success)
        {
            InitializeComponent(); // Initialisiert die Komponenten
            _imodel = FindResource("imodel") as ImportModel ?? new(); // Findet oder erstellt eine neue ImportModel-Instanz
            _model = FindResource("pmodel") as MainModel ?? new(); // Findet oder erstellt eine neue MainModel-Instanz
            DataContext = _imodel; // Setzt den DataContext auf das ImportModel

            // Event-Handler für den Schließen-Button
            btnSchliessen.Click += (s, e) =>
            {
                // Topbar wieder aktivieren
                if (Application.Current.MainWindow is Main mainWindow)
                {
                    mainWindow.SetTopBarEnabled(true);
                }

                if (tbTitel.Text == "Fehler")
                {
                    _model.Navigate(_model.History[^1]); // Navigiert zur letzten Seite im Verlauf bei Fehler
                }
                else
                {
                    Object page = _model.History.FindLast(x => x.GetType() == typeof(Dateiverwaltung)); // Findet die letzte Dateiverwaltung-Seite im Verlauf
                    _model.Navigate(page); // Navigiert zur gefundenen Seite
                }
            };

            //btnWeitereDatei.Click += btnWeitereDatei_Click; // Event-Handler für den Weitere-Datei-Button

            // Setzt die UI-Elemente basierend auf dem Erfolg
            if (success)
            {
                tbTitel.Text = "Erfolg"; // Setzt den Titel auf "Erfolg"
                tbMeldung.Text = meldung; // Setzt die Meldung
                tbTitel.Foreground = Brushes.Green; // Setzt die Schriftfarbe auf Grün
                btnSchliessen.Content = "Schließen"; // Setzt den Button-Text auf "Schließen"
                //btnWeitereDatei.Visibility = Visibility.Visible; // Macht den Weitere-Datei-Button sichtbar
            }
            else
            {
                tbTitel.Foreground = Brushes.Red; // Setzt die Schriftfarbe auf Rot
                tbTitel.Text = "Fehler"; // Setzt den Titel auf "Fehler"
                tbMeldung.Text = meldung; // Setzt die Meldung
                btnSchliessen.Content = "Zurück"; // Setzt den Button-Text auf "Zurück"
            }
        }

        //// Event-Handler für den Weitere-Datei-Button
        //private void btnWeitereDatei_Click(object sender, RoutedEventArgs e)
        //{
        //    _model.Navigate(_model.History.FindLast(x => x.GetType() == typeof(Dateiverwaltung))); // Navigiert zur letzten Dateiverwaltung-Seite im Verlauf
        //    Application.Current.Dispatcher.InvokeAsync(() =>
        //    {
        //        var dateiverwaltungPage = _model.CurrentPage as Dateiverwaltung; // Holt die aktuelle Seite als Dateiverwaltung
        //        dateiverwaltungPage?.ShowRemainingFiles(); // Zeigt verbleibende Dateien an, falls vorhanden
        //    });
        //}
    }
}
