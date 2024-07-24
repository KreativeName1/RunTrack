using System.Windows;
using System.Windows.Controls;

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
    }
}
