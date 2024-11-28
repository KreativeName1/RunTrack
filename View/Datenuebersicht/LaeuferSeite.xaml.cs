using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für SchuelerSeite.xaml
    /// </summary>
    public partial class LaeuferSeite : Page
    {
        private LaeuferseiteModel _model;
        private MainModel _mmodel;
        private bool _isUserInteraction = false;

        public LaeuferSeite()
        {
            InitializeComponent();

            _model = FindResource("thismodel") as LaeuferseiteModel;
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

                var neuerLaeufer = new Laeufer();
                neuerLaeufer.Geburtsjahrgang = 2000;

                _model.LstLaeufer.Add(neuerLaeufer);
                _model.SelLaeufer = neuerLaeufer; // Setze den neuen Schüler als ausgewählt
                lstLaeufer.SelectedItem = neuerLaeufer; // Stelle sicher, dass er im DataGrid ausgewählt ist
                lstLaeufer.ScrollIntoView(neuerLaeufer); // Scrolle zum neuen Eintrag

                // Fokus auf das DataGrid setzen
                lstLaeufer.Focus();

                // Dispatcher verwenden, um die Bearbeitung zu aktivieren, nachdem alle Ereignisse verarbeitet sind
                Dispatcher.InvokeAsync(() =>
                {
                    var firstEditableColumn = lstLaeufer.Columns.FirstOrDefault(col => !col.IsReadOnly);
                    if (firstEditableColumn != null)
                    {
                        lstLaeufer.CurrentCell = new DataGridCellInfo(neuerLaeufer, firstEditableColumn);
                        lstLaeufer.BeginEdit();
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

                if (_model.SelLaeufer.Id == null)
                {
                    message = "Möchten Sie diesen Eintrag wirklich löschen?";
                }
                else
                {
                    message = $"Möchten Sie diesen Eintrag wirklich löschen? \n- {_model.SelLaeufer.Id}:\t{_model.SelLaeufer.Nachname}, {_model.SelLaeufer.Vorname}";
                }

                var res = new Popup().Display("Löschen", message, PopupType.Question, PopupButtons.YesNo);

                if (res == true)
                {
                    _model.LstLaeufer.Remove(_model.SelLaeufer);
                    _model.HasChanges = true;
                }


                _model.LstLaeufer.Remove(_model.SelLaeufer);
                _model.HasChanges = true;
            };

            btnDruck.Click += (s, e) =>
            {
                List<Laeufer> list = new();

                foreach (Laeufer laeufer in lstLaeufer.SelectedItems) list.Add(laeufer);

                if (list.Count == 0)
                {
                    new Popup().Display("Fehler", "Keine Läufer ausgewählt", PopupType.Error, PopupButtons.Ok);
                    return;
                }

                PDFEditor editor = new(list);
                _mmodel.Navigate(editor); 
            };

            lstLaeufer.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    if (_model.SelLaeufer.Geburtsjahrgang < 1900)
                    {
                        _model.SelLaeufer.Geburtsjahrgang = 1900;
                    }
                    _model.HasChanges = true;
                }
            };
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstLaeufer, false);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstLaeufer, true);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }


        private void cbRundenArt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model.SelLaeufer == null) return;
            ComboBox cbRundenArt = sender as ComboBox;
            _model.HasChanges = true;
            RundenArt rundenArt = cbRundenArt.SelectedItem as RundenArt;

            if (rundenArt != null)
            {
                _model.SelLaeufer.RundenArt = rundenArt;
                _model.SelLaeufer.RundenArtId = rundenArt.Id;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUserInteraction)
            {
                _model.HasChanges = true;

                ComboBox cb = sender as ComboBox;
                Laeufer laeufer = cb.DataContext as Laeufer;

                if (laeufer != null)
                {
                    laeufer.Geschlecht = (Geschlecht)cb.SelectedItem;
                }
            }
        }

        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            _isUserInteraction = !_isUserInteraction;
        }
    }
}
