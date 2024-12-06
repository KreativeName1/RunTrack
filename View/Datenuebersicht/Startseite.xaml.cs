using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack.View.Datenuebersicht
{
    public partial class Startseite : Page
    {
        private StartseiteModel _model;

        // Konstruktor der Startseite-Klasse
        public Startseite()
        {
            InitializeComponent();

            // Initialisiert das Modell, indem es eine Ressource mit dem Schlüssel "thismodel" sucht
            _model = FindResource("thismodel") as StartseiteModel;

            // Fügt ein Ereignis hinzu, das beim Entladen der Seite ausgelöst wird
            this.Unloaded += (s, e) =>
            {
                // Schließt die Datenbankverbindung, wenn die Seite entladen wird
                _model.Db.Dispose();
            };
        }

        // Ereignishandler für den Klick auf den "Up"-Button
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            // Wählt die vorherige Zeile in der Liste aus
            UebersichtMethoden.SelectSearchedRow(lstStartseite, false);
        }

        // Ereignishandler für den Klick auf den "Down"-Button
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            // Wählt die nächste Zeile in der Liste aus
            UebersichtMethoden.SelectSearchedRow(lstStartseite, true);
        }

        // Ereignishandler, wenn das Textfeld den Fokus verliert
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            // Setzt die Vordergrundfarbe des Textfeldes auf Blau
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
    }
}
