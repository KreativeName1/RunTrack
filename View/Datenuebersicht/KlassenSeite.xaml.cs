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
    }
}
