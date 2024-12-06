using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack.View.Datenuebersicht
{
    /// <summary>
    /// Interaktionslogik für RundenSeite.xaml
    /// </summary>
    public partial class RundenSeite : Page
    {
        private RundenseiteModel _model;

        // Konstruktor der RundenSeite-Klasse
        public RundenSeite()
        {
            InitializeComponent();
            // Initialisiere das Model und den Datenbankkontext
            _model = FindResource("thismodel") as RundenseiteModel;
            _model.Db = new LaufDBContext();

            // Event-Handler für das Unloaded-Ereignis
            this.Unloaded += (s, e) =>
            {
                // Dispose des Datenbankkontexts und Zurücksetzen der Änderungen
                _model.Db.Dispose();
                _model.HasChanges = false;
            };

            // Event-Handler für den Klick auf den Speichern-Button
            this.btnSpeichern.Click += (s, e) =>
            {
                try
                {
                    // Speichern der Änderungen im Model
                    _model.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Anzeige eines Fehlers, falls das Speichern fehlschlägt
                    new Popup().Display("Fehler beim Speichern", ex.Message, PopupType.Error, PopupButtons.Ok);
                }
            };

            // Event-Handler für den Klick auf den Löschen-Button
            this.btnDel.Click += (s, e) =>
            {

                string message = "";

                // Überprüfen, ob eine Runde ausgewählt ist
                if (_model.SelRunde.Id == null)
                {
                    message = "Möchten Sie diesen Eintrag wirklich löschen?";
                }
                else
                {
                    message = $"Möchten Sie diesen Eintrag wirklich löschen? \n- {_model.SelRunde.Id}:\t{_model.SelRunde.Laeufer.Nachname}, {_model.SelRunde.Laeufer.Vorname}";
                }

                // Anzeige eines Bestätigungs-Popups
                var res = new Popup().Display("Löschen", message, PopupType.Question, PopupButtons.YesNo);

                // Löschen der ausgewählten Runde, falls bestätigt
                if (res == true)
                {
                    _model.LstRunde.Remove(_model.SelRunde);
                    _model.HasChanges = true;
                }
            };
        }

        // Event-Handler für den Klick auf den Hoch-Button
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstRunden, false);
        }

        // Event-Handler für den Klick auf den Runter-Button
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstRunden, true);
        }

        // Event-Handler für das LostFocus-Ereignis des Suchfeldes
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            // Ändern der Textfarbe des Suchfeldes
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
    }
}
