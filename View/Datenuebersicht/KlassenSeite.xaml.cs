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
        private DatenuebersichtModel _model;
        private ScannerModel _mainViewModel;
        private MainModel _pmodel;

        private LaufDBContext _db = new();
        public KlassenSeite()
        {
            InitializeComponent();
            _mainViewModel = FindResource("smodel") as ScannerModel ?? new ScannerModel();
            _model = FindResource("dumodel") as DatenuebersichtModel ?? new DatenuebersichtModel();
            _pmodel = FindResource("pmodel") as MainModel ?? new MainModel();
            btnBarcodes.Click += (sender, e) =>
            {
                PDFEditor pdfEditor = new(_model.SelKlasse ?? new());
                _pmodel.Navigate(pdfEditor);
            };


            _model = FindResource("dumodel") as DatenuebersichtModel ?? new();
            btnNeu.Click += (sender, e) =>
            {
                Klasse neu = new();
                _db.Klassen.Add(neu);
                _model.LstKlasse.Add(neu);

                lstKlasse.ScrollIntoView(neu);
                lstKlasse.SelectedItem = neu;
                lstKlasse.Focus();

            };
            btnSpeichern.Click += (sender, e) =>
            {
                _db.SaveChanges();
            };

            btnDel.Click += (sender, e) =>
            {
                Klasse klasse = lstKlasse.SelectedItem as Klasse;
                if (klasse == null) return;
                _model.LstKlasse.Remove(klasse);
                Klasse dbKlasse = _db.Klassen.Find(klasse.Id);
                if (dbKlasse != null)
                {
                    _db.Klassen.Remove(dbKlasse);
                    _db.SaveChanges();
                }
            };
            lstKlasse.CellEditEnding += (sender, e) =>
            {
                Klasse klasse = lstKlasse.SelectedItem as Klasse;
                if (klasse == null) return;
                Klasse dbKlasse = _db.Klassen.Find(klasse.Id);
                if (dbKlasse != null)
                {
                    dbKlasse.Name = klasse.Name;
                    _db.SaveChanges();
                }
            };

            if (this.Visibility != Visibility)
                _db.Dispose();
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
            ComboBox cbSchule = sender as ComboBox;

            Klasse sel = _model.SelKlasse;
            if (sel == null) return;
            Klasse klasse = _db.Klassen.Find(sel.Id);

            _model.SelSchule = cbSchule.SelectedItem as Schule;

            sel.Schule = klasse.Schule = cbSchule.SelectedItem as Schule;
            // sel.Schule = klasse.Schule;

            _db.SaveChanges();
            // cbSchule.SelectedItem = klasse.Schule;

            _model.SelKlasse = null;

        }

        private void cbRundenArt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox checkBox = sender as ComboBox;

            Klasse sel = _model.SelKlasse;

            if (sel == null)
                return;

            Klasse klasse = _db.Klassen.Find(sel.Id);

            RundenArt rundenArt = checkBox.SelectedItem as RundenArt;

            _model.SelRundenArt = rundenArt;
            sel.RundenArt = klasse.RundenArt = rundenArt;
            // sel.RundenArt = klasse.RundenArt;

            _db.SaveChanges();
            checkBox.SelectedItem = klasse.RundenArt;

            _model.SelKlasse = null;
        }
    }
}
