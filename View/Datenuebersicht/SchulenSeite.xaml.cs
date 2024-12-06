using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Schulen.xaml
    /// </summary>
    public partial class SchulenSeite : Page
    {
        private SchulenseiteModel _model;

        public SchulenSeite()
        {
            InitializeComponent();
            // Initialisiere das Modell
            _model = FindResource("thismodel") as SchulenseiteModel ?? new();

            // Ereignis beim Entladen der Seite
            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };

            // Ereignis beim Klicken auf den "Neu" Button
            btnNeu.Click += (sender, e) =>
            {
                txtSearch.Text = "";
                txtSearch.IsEnabled = false;

                var neueSchule = new Schule();

                _model.LstSchule.Add(neueSchule);
                _model.SelSchule = neueSchule; // Setze die neue Schule als ausgewählt
                lstSchule.SelectedItem = neueSchule; // Stelle sicher, dass sie im DataGrid ausgewählt ist
                lstSchule.ScrollIntoView(neueSchule); // Scrolle zum neuen Eintrag

                // Fokus auf das DataGrid setzen
                lstSchule.Focus();

                // Dispatcher verwenden, um die Bearbeitung zu aktivieren, nachdem alle Ereignisse verarbeitet sind
                Dispatcher.InvokeAsync(() =>
                {
                    var firstEditableColumn = lstSchule.Columns.FirstOrDefault(col => !col.IsReadOnly);
                    if (firstEditableColumn != null)
                    {
                        lstSchule.CurrentCell = new DataGridCellInfo(neueSchule, firstEditableColumn);
                        lstSchule.BeginEdit();
                    }
                });

                _model.HasChanges = true;
            };

            // Ereignis beim Klicken auf den "Speichern" Button
            btnSpeichern.Click += (sender, e) =>
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

            // Ereignis beim Klicken auf den "Löschen" Button
            btnDel.Click += (sender, e) =>
            {
                string message = "";

                if (_model.SelSchule.Id == null)
                {
                    message = "Möchten Sie diesen Eintrag wirklich löschen?";
                }
                else
                {
                    message = $"Möchten Sie diesen Eintrag wirklich löschen? \n- {_model.SelSchule.Id}:\t{_model.SelSchule.Name}";
                }

                var res = new Popup().Display("Löschen", message, PopupType.Question, PopupButtons.YesNo);

                if (res == true)
                {
                    _model.LstSchule.Remove(_model.SelSchule);
                    _model.HasChanges = true;
                }
            };

            // Ereignis beim Beenden der Zellenbearbeitung im DataGrid
            lstSchule.CellEditEnding += (sender, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true;
            };
        }

        // Ereignis beim Klicken auf den "Up" Button
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchule, false);
        }

        // Ereignis beim Klicken auf den "Down" Button
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchule, true);
        }

        // Ereignis beim Verlassen des Suchfeldes
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
    }
}