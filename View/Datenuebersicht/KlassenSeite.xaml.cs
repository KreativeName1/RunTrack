using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Klimalauf.View.Datenuebersicht
{
    /// <summary>
    /// Interaktionslogik für KlassenSeite.xaml
    /// </summary>
    public partial class KlassenSeite : Page
    {
        private DatenuebersichtModel _model;
        private MainViewModel _mainViewModel;
        public KlassenSeite()
        {
            InitializeComponent();
            _mainViewModel = FindResource("mvmodel") as MainViewModel ?? new MainViewModel();
            _model = FindResource("dumodel") as DatenuebersichtModel ?? new DatenuebersichtModel();
            btnBarcodes.Click += (sender, e) =>
            {
                PDFEditor pdfEditor = new PDFEditor(_model.SelKlasse ?? new());
                Window.GetWindow(this).Hide();
                _mainViewModel.LastWindow = Window.GetWindow(this);
                pdfEditor.Show();
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
