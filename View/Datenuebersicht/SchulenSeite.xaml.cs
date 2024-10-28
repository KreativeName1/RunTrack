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
            _model = FindResource("thismodel") as SchulenseiteModel ?? new();

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };
            btnNeu.Click += (sender, e) =>
            {
                Schule neu = new();
                _model.LstSchule.Add(neu);
                _model.HasChanges = true;
                lstSchule.ScrollIntoView(neu);

            };
            btnSpeichern.Click += (sender, e) =>
            {
                try
                {
                    _model.SaveChanges();
                }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler beim Speichern", ex.Message, PopupType.Error, PopupButtons.Ok);
                }
            };

            btnDel.Click += (sender, e) =>
            {
                _model.LstSchule.Remove(_model.SelSchule);
                _model.HasChanges = true;
            };
            lstSchule.CellEditEnding += (sender, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true;
            };

        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UebersichtMethoden.SearchDataGrid(lstSchule, txtSearch.Text);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchule, false, txtSearch.Text);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchule, true, txtSearch.Text);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
    }
}