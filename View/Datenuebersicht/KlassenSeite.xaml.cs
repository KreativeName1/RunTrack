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
        private KlassenseiteModel _model;
        private MainModel _mmodel;
        private bool _isUserInteractionSchule = false;
        private bool _isUserInteractionRundenart = false;
        public KlassenSeite()
        {
            InitializeComponent();
            _model = FindResource("thismodel") as KlassenseiteModel;
            _mmodel = FindResource("pmodel") as MainModel;

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };

            btnNeu.Click += (s, e) =>
            {
                txtSearch.Text = "";
                txtSearch.IsEnabled = false;

                var neueKlasse = new Klasse();

                _model.LstKlasse.Add(neueKlasse);
                _model.SelKlasse = neueKlasse; // Setze den neuen Schüler als ausgewählt
                lstKlasse.SelectedItem = neueKlasse; // Stelle sicher, dass er im DataGrid ausgewählt ist
                lstKlasse.ScrollIntoView(neueKlasse); // Scrolle zum neuen Eintrag

                // Fokus auf das DataGrid setzen
                lstKlasse.Focus();

                // Dispatcher verwenden, um die Bearbeitung zu aktivieren, nachdem alle Ereignisse verarbeitet sind
                Dispatcher.InvokeAsync(() =>
                {
                    var firstEditableColumn = lstKlasse.Columns.FirstOrDefault(col => !col.IsReadOnly);
                    if (firstEditableColumn != null)
                    {
                        lstKlasse.CurrentCell = new DataGridCellInfo(neueKlasse, firstEditableColumn);
                        lstKlasse.BeginEdit();
                    }
                });

                _model.HasChanges = true;
            };

            btnSpeichern.Click += (s, e) =>
            {
                txtSearch.IsEnabled = true;

                try
                {
                    _model.SaveChanges();
                }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler beim Speichern", ex.Message, PopupType.Error, PopupButtons.Ok);
                }
            };

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
                    _model.LstKlasse.Remove(_model.SelKlasse);
                    _model.HasChanges = true;
                }
            };

            lstKlasse.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true;
            };

            btnBarcodes.Click += (sender, e) =>
            {
              if (_model.SelKlasse == null)
                {
                    new Popup().Display("Fehler", "Bitte wählen Sie eine Klasse aus", PopupType.Error, PopupButtons.Ok);
                    return;
                }
                PDFEditor pdfEditor = new(_model.SelKlasse ?? new());
                _mmodel.Navigate(pdfEditor);
            };
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstKlasse, false);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstKlasse, true);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
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

        private void cbKlasse_DropDownOpened(object sender, EventArgs e)
        {
            _isUserInteractionSchule = true;
        }

        private void cbKlasse_DropDownClosed(object sender, EventArgs e)
        {
            _isUserInteractionSchule = false;
        }

        private void cbKlasse_DropDownOpened1(object sender, EventArgs e)
        {
            _isUserInteractionRundenart = true;
        }

        private void cbKlasse_DropDownClosed1(object sender, EventArgs e)
        {
            _isUserInteractionRundenart = false;
        }
    }
}
