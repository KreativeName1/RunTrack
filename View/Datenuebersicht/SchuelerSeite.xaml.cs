using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für SchuelerSeite.xaml
    /// </summary>
    public partial class SchuelerSeite : Page
    {
        private SchuelerseiteModel _model;
        private bool _isUserInteraction = false;

        public SchuelerSeite()
        {
            InitializeComponent();
            _model = FindResource("thismodel") as SchuelerseiteModel;

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };
            btnNeu.Click += (s, e) =>
            {
                var neuerSchueler = new Schueler();
                _model.LstSchueler.Add(neuerSchueler);
                _model.SelSchueler = neuerSchueler; // Setze den neuen Schüler als ausgewählt
                lstSchueler.SelectedItem = neuerSchueler; // Stelle sicher, dass er im DataGrid ausgewählt ist
                lstSchueler.ScrollIntoView(neuerSchueler); // Scrolle zum neuen Eintrag
                _model.HasChanges = true;
            };

            btnSpeichern.Click += (s, e) =>
            {
                try { _model.SaveChanges(); }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler beim Speichern", ex.Message, PopupType.Error, PopupButtons.Ok);
                }
            };
            btnDel.Click += (s, e) =>
            {
                _model.LstSchueler.Remove(_model.SelSchueler);
                _model.HasChanges = true;
            };

            lstSchueler.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true;
            };

        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, false);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, true);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }

        private void cbSchule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model.HasChanges = true;
        }
        private void cbKlasse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model.SelSchueler == null) return;

            if (_isUserInteraction)
            {
                _model.HasChanges = true;
                _isUserInteraction = false;
                ComboBox cbKlasse = sender as ComboBox;
                Klasse klasse = cbKlasse.SelectedItem as Klasse;

                if (klasse != null)
                {
                    _model.SelSchueler.Klasse = klasse;
                    _model.SelSchueler.KlasseId = klasse.Id;
                }
            }

        }

        private void cbKlasse_DropDownOpened(object sender, EventArgs e)
        {
            _isUserInteraction = true;
        }

        private void cbKlasse_DropDownClosed(object sender, EventArgs e)
        {
            _isUserInteraction = false;
        }
    }
}
