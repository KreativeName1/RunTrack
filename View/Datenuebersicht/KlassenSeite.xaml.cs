using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack.View.Datenuebersicht
{
    /// <summary>
    /// Interaktionslogik für KlassenSeite.xaml
    /// </summary>
    public partial class KlassenSeite : Page
    {
        private KlassenseiteModel _model; // Modell für die Klassenseite
        private MainModel _mmodel; // Hauptmodell
        private bool _isUserInteractionSchule = false; // Benutzerinteraktion für Schule
        private bool _isUserInteractionRundenart = false; // Benutzerinteraktion für Rundenart

        public KlassenSeite()
        {
            InitializeComponent();
            _model = FindResource("thismodel") as KlassenseiteModel; // Modellressource finden
            _mmodel = FindResource("pmodel") as MainModel; // Hauptmodellressource finden

            // Ereignis beim Entladen der Seite
            this.Unloaded += (s, e) =>
            {
                _model.HasChanges = false; // Änderungen zurücksetzen
            };

            // Ereignis beim Klicken auf den "Neu" Button
            btnNeu.Click += (s, e) =>
            {
                txtSearch.Text = "";
                txtSearch.IsEnabled = false;

                var neueKlasse = new Klasse(); // Neue Klasse erstellen

                _model.LstKlasse.Add(neueKlasse); // Neue Klasse zur Liste hinzufügen
                _model.SelKlasse = neueKlasse; // Neue Klasse als ausgewählt setzen
                lstKlasse.SelectedItem = neueKlasse; // Neue Klasse im DataGrid auswählen
                lstKlasse.ScrollIntoView(neueKlasse); // Zum neuen Eintrag scrollen

                lstKlasse.Focus(); // Fokus auf das DataGrid setzen

                // Bearbeitung aktivieren, nachdem alle Ereignisse verarbeitet sind
                Dispatcher.InvokeAsync(() =>
                {
                    var firstEditableColumn = lstKlasse.Columns.FirstOrDefault(col => !col.IsReadOnly);
                    if (firstEditableColumn != null)
                    {
                        lstKlasse.CurrentCell = new DataGridCellInfo(neueKlasse, firstEditableColumn);
                        lstKlasse.BeginEdit();
                    }
                });

                _model.HasChanges = true; // Änderungen markieren
            };

            // Ereignis beim Klicken auf den "Speichern" Button
            btnSpeichern.Click += (s, e) =>
            {
                txtSearch.IsEnabled = true;

                try
                {
                    _model.SaveChanges(); // Änderungen speichern
                }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler beim Speichern", ex.Message, PopupType.Error, PopupButtons.Ok);
                }
            };

            // Ereignis beim Klicken auf den "Löschen" Button
            btnDel.Click += (s, e) =>
            {
                string message = "";

                if (_model.SelKlasse.Id == null)
                {
                    message = "Möchten Sie diesen Eintrag wirklich löschen?";
                }
                else
                {
                    message = $"Möchten Sie diesen Eintrag wirklich löschen? \n- {_model.SelKlasse.Id}:\t{_model.SelKlasse.Name}";
                }

                var res = new Popup().Display("Löschen", message, PopupType.Question, PopupButtons.YesNo);

                if (res == true)
                {
                    _model.LstKlasse.Remove(_model.SelKlasse); // Ausgewählte Klasse entfernen
                    _model.HasChanges = true; // Änderungen markieren
                }
            };

            // Ereignis beim Beenden der Zellenbearbeitung im DataGrid
            lstKlasse.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true; // Änderungen markieren
            };

            // Ereignis beim Klicken auf den "Barcodes" Button
            btnBarcodes.Click += (sender, e) =>
            {
                if (_model.SelKlasse == null)
                {
                    new Popup().Display("Fehler", "Bitte wählen Sie eine Klasse aus", PopupType.Error, PopupButtons.Ok);
                    return;
                }

                List<Klasse> liste = new();
                foreach (Klasse klasse in lstKlasse.SelectedItems) liste.Add(klasse);

                PDFEditor pdfEditor = new(liste ?? new());
                _mmodel.Navigate(pdfEditor); // Navigation zum PDF-Editor
            };
        }

        // Ereignis beim Klicken auf den "Up" Button
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstKlasse, false);
        }

        // Ereignis beim Klicken auf den "Down" Button
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstKlasse, true);
        }

        // Ereignis beim Verlassen des Suchfeldes
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }

        // Ereignis beim Ändern der Auswahl in der Schule-ComboBox
        private void cbSchule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUserInteractionSchule)
            {
                _model.HasChanges = true;
                _isUserInteractionSchule = false;
                ComboBox cbSchule = sender as ComboBox;
                Schule schule = cbSchule.SelectedItem as Schule;

                if (schule != null)
                {
                    _model.SelKlasse.Schule = schule;
                    _model.SelKlasse.SchuleId = schule.Id;
                }
            }
        }

        // Ereignis beim Ändern der Auswahl in der RundenArt-ComboBox
        private void cbRundenArt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model.SelKlasse == null) return;

            if (_isUserInteractionRundenart)
            {
                _model.HasChanges = true;
                _isUserInteractionRundenart = false;
                ComboBox cbRundenArt = sender as ComboBox;
                RundenArt rundenArt = cbRundenArt.SelectedItem as RundenArt;

                if (rundenArt != null)
                {
                    _model.SelKlasse.RundenArt = rundenArt;
                    _model.SelKlasse.RundenArtId = rundenArt.Id;
                }
            }
        }

        // Ereignis beim Öffnen der Schule-ComboBox
        private void cbKlasse_DropDownOpened(object sender, EventArgs e)
        {
            _isUserInteractionSchule = true;
        }

        // Ereignis beim Schließen der Schule-ComboBox
        private void cbKlasse_DropDownClosed(object sender, EventArgs e)
        {
            _isUserInteractionSchule = false;
        }

        // Ereignis beim Öffnen der RundenArt-ComboBox
        private void cbKlasse_DropDownOpened1(object sender, EventArgs e)
        {
            _isUserInteractionRundenart = true;
        }

        // Ereignis beim Schließen der RundenArt-ComboBox
        private void cbKlasse_DropDownClosed1(object sender, EventArgs e)
        {
            _isUserInteractionRundenart = false;
        }
    }
}
