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
                _model.LstSchueler.Add(new Schueler());
                _model.HasChanges = true;
            };
            btnSpeichern.Click += (s, e) => { _model.SaveChanges(); };
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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UebersichtMethoden.SearchDataGrid(lstSchueler, txtSearch.Text);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, false, txtSearch.Text);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, true, txtSearch.Text);
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
            ComboBox cbKlasse = sender as ComboBox;
            _model.HasChanges = true;
            Klasse klasse = cbKlasse.SelectedItem as Klasse;

            if (klasse != null)
            {
                _model.SelSchueler.Klasse = klasse;
                _model.SelSchueler.KlasseId = klasse.Id;
            }

        }
    }
}
