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

            // Initialisiere das Model und das Hauptmodel
            _model = FindResource("thismodel") as LaeuferseiteModel;
            _mmodel = FindResource("pmodel") as MainModel;

            // Setze HasChanges auf false, wenn die Seite entladen wird
            this.Unloaded += (s, e) =>
            {
                _model.HasChanges = false;
            };

            // Event-Handler für den "Neu"-Button
            btnNeu.Click += (s, e) =>
            {
                txtSearch.Text = "";
                txtSearch.IsEnabled = false;

                var neuerLaeufer = new Laeufer();
                neuerLaeufer.Geburtsjahrgang = 2000;

                _model.LstLaeufer.Add(neuerLaeufer);
                _model.SelLaeufer = neuerLaeufer; // Setze den neuen Läufer als ausgewählt
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

            // Event-Handler für den "Speichern"-Button
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

            // Event-Handler für den "Löschen"-Button
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
            };

            // Event-Handler für den "Druck"-Button
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

            // Event-Handler für das Ende der Zellenbearbeitung im DataGrid
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

        // Event-Handler für den "Up"-Button
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstLaeufer, false);
        }

        // Event-Handler für den "Down"-Button
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstLaeufer, true);
        }

        // Event-Handler für das Verlassen des Suchfeldes
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }

        // Event-Handler für die Auswahländerung in der ComboBox für RundenArt
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

        // Event-Handler für die Auswahländerung in der ComboBox für Geschlecht
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

        // Event-Handler für das Öffnen des Dropdowns in der ComboBox
        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            _isUserInteraction = !_isUserInteraction;
        }
    }
}
