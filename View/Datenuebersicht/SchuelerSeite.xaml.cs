using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für SchuelerSeite.xaml
    /// </summary>
    public partial class SchuelerSeite : Page
    {
        private SchuelerseiteModel _model; // Modell für die Schülerseite
        private bool _isUserInteraction = false; // Flag für Benutzerinteraktion
        private bool _isUserInteractionGeschlecht = false; // Flag für Benutzerinteraktion bei Geschlecht

        public SchuelerSeite()
        {
            InitializeComponent();
            _model = FindResource("thismodel") as SchuelerseiteModel; // Initialisiert das Modell

            // Ereignis beim Entladen der Seite
            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose(); // Schließt die Datenbankverbindung
                _model.HasChanges = false; // Setzt Änderungen-Flag zurück
            };

            // Ereignis beim Klicken auf den "Neu" Button
            btnNeu.Click += (s, e) =>
            {
                txtSearch.Text = "";
                txtSearch.IsEnabled = false;

                var neuerSchueler = new Schueler();
                neuerSchueler.Geburtsjahrgang = 2000;

                _model.LstSchueler.Add(neuerSchueler); // Fügt neuen Schüler zur Liste hinzu
                _model.SelSchueler = neuerSchueler; // Setzt den neuen Schüler als ausgewählt
                lstSchueler.SelectedItem = neuerSchueler; // Wählt den neuen Schüler im DataGrid aus
                lstSchueler.ScrollIntoView(neuerSchueler); // Scrollt zum neuen Eintrag

                // Setzt den Fokus auf das DataGrid
                lstSchueler.Focus();

                // Aktiviert die Bearbeitung nach Verarbeitung aller Ereignisse
                Dispatcher.InvokeAsync(() =>
                {
                    var firstEditableColumn = lstSchueler.Columns.FirstOrDefault(col => !col.IsReadOnly);
                    if (firstEditableColumn != null)
                    {
                        lstSchueler.CurrentCell = new DataGridCellInfo(neuerSchueler, firstEditableColumn);
                        lstSchueler.BeginEdit();
                    }
                });

                _model.HasChanges = true; // Setzt Änderungen-Flag
            };

            // Ereignis beim Klicken auf den "Speichern" Button
            btnSpeichern.Click += (s, e) =>
            {
                txtSearch.IsEnabled = true;

                try
                {
                    _model.SaveChanges(); // Speichert Änderungen
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

                if (_model.SelSchueler.Id == null)
                {
                    message = "Möchten Sie diesen Eintrag wirklich löschen?";
                }
                else
                {
                    message = $"Möchten Sie diesen Eintrag wirklich löschen? \n- {_model.SelSchueler.Id}:\t{_model.SelSchueler.Nachname}, {_model.SelSchueler.Vorname}";
                }

                var res = new Popup().Display("Löschen", message, PopupType.Question, PopupButtons.YesNo);

                if (res == true)
                {
                    _model.LstSchueler.Remove(_model.SelSchueler); // Entfernt den ausgewählten Schüler aus der Liste
                    _model.HasChanges = true; // Setzt Änderungen-Flag
                }
            };

            // Ereignis beim Beenden der Zellenbearbeitung im DataGrid
            lstSchueler.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    if (_model.SelSchueler.Geburtsjahrgang < 1900)
                    {
                        _model.SelSchueler.Geburtsjahrgang = 1900; // Setzt das Geburtsjahr auf 1900, wenn es kleiner ist
                    }
                    _model.HasChanges = true; // Setzt Änderungen-Flag
                }
            };
        }

        // Ereignis beim Klicken auf den "Up" Button
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, false);
        }

        // Ereignis beim Klicken auf den "Down" Button
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, true);
        }

        // Ereignis beim Verlassen des Suchfeldes
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }

        // Ereignis bei Änderung der Schulauswahl
        private void cbSchule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model.HasChanges = true; // Setzt Änderungen-Flag
        }

        // Ereignis bei Änderung der Klassenauswahl
        private void cbKlasse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUserInteraction)
            {
                _model.HasChanges = true; // Setzt Änderungen-Flag
                _isUserInteraction = false;
                ComboBox cbKlasse = sender as ComboBox;
                Klasse klasse = cbKlasse.SelectedItem as Klasse;

                if (klasse != null)
                {
                    _model.SelSchueler.Klasse = klasse; // Setzt die ausgewählte Klasse
                    _model.SelSchueler.KlasseId = klasse.Id; // Setzt die Klassen-ID
                }
            }
        }

        // Ereignis beim Öffnen des Klassen-Dropdowns
        private void cbKlasse_DropDown(object sender, EventArgs e)
        {
            _isUserInteraction = !_isUserInteraction; // Wechselt das Benutzerinteraktions-Flag
        }

        // Ereignis beim Öffnen des Geschlecht-Dropdowns
        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            _isUserInteractionGeschlecht = !_isUserInteractionGeschlecht; // Wechselt das Benutzerinteraktions-Flag für Geschlecht
        }

        // Ereignis bei Änderung der Geschlechtsauswahl
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUserInteractionGeschlecht)
            {
                _model.HasChanges = true; // Setzt Änderungen-Flag
                ComboBox cb = sender as ComboBox;
                Schueler schueler = cb.DataContext as Schueler;
                if (schueler != null)
                {
                    schueler.Geschlecht = (Geschlecht)cb.SelectedItem; // Setzt das ausgewählte Geschlecht
                }
            }
        }

        // Ereignis beim Laden des ComboBox
        private void comboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is Schueler schueler)
            {
                Trace.WriteLine(_model.LstSchueler);
                ListCollectionView view = new(_model.LstKlasse);
                view.GroupDescriptions.Add(new PropertyGroupDescription("Schule.Name"));
                comboBox.ItemsSource = view;

                if (schueler.Klasse != null)
                {
                    comboBox.SelectedItem = schueler.Klasse; // Setzt die ausgewählte Klasse
                }
            }
        }

        // Ereignis beim Schließen des Klassen-Dropdowns
        private void cbKlase_DropDownClosed(object sender, EventArgs e)
        {
        }

        // Ereignis beim Öffnen des Klassen-Dropdowns
        private void cbKlase_DropDownOpened(object sender, EventArgs e)
        {
            _isUserInteraction = true; // Setzt das Benutzerinteraktions-Flag
        }
    }
}