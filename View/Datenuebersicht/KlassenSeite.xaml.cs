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
                _model.LstKlasse.Add(new Klasse());
                _model.HasChanges = true;
            };
            btnSpeichern.Click += (s, e) => { _model.SaveChanges(); };
            btnDel.Click += (s, e) =>
            {
                _model.LstKlasse.Remove(_model.SelKlasse);
                _model.HasChanges = true;
            };

            lstKlasse.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true;
            };

            btnBarcodes.Click += (sender, e) =>
            {
                PDFEditor pdfEditor = new(_model.SelKlasse ?? new());
                _mmodel.Navigate(pdfEditor);
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UebersichtMethoden.SearchDataGrid(lstKlasse, txtSearch.Text);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstKlasse, false, txtSearch.Text);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstKlasse, true, txtSearch.Text);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
        private void cbSchule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model.SelKlasse == null) return;
            ComboBox cbSchule = sender as ComboBox;
            _model.HasChanges = true;
            Schule schule = cbSchule.SelectedItem as Schule;

            if (schule != null)
            {
                _model.SelKlasse.Schule = schule;
                _model.SelKlasse.SchuleId = schule.Id;
            }
        }

        private void cbRundenArt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model.SelKlasse == null) return;
            ComboBox cbRundenArt = sender as ComboBox;
            _model.HasChanges = true;
            RundenArt rundenArt = cbRundenArt.SelectedItem as RundenArt;

            if (rundenArt != null)
            {
                _model.SelKlasse.RundenArt = rundenArt;
                _model.SelKlasse.RundenArtId = rundenArt.Id;
            }
        }
    }
}
