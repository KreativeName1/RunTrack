using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für ImportFenster.xaml
    /// </summary>
    public partial class ImportFenster : Page
    {
        private ImportModel _imodel;
        private PageModel _pmodel;
        public ImportFenster(string pfad)
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new ImportModel();
            _pmodel = FindResource("pmodel") as PageModel ?? new PageModel();
            _imodel.Pfad = pfad;
            DataContext = _imodel;
            _imodel.ShowImport1();

            _imodel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CurrentView")
                {
                    if (_imodel.CurrentView == null)
                    {
                        _pmodel.Navigate(_pmodel.History[^1]);
                    }
                }
            };

        }
    }
}
