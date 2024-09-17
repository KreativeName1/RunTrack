using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für ImportFenster.xaml
    /// </summary>
    public partial class ImportFenster : Window
    {
        private ImportModel _imodel;
        private MainModel _pmodel;
        public ImportFenster(string pfad)
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new ImportModel();
            _pmodel = FindResource("pmodel") as MainModel ?? new MainModel();
            _imodel.Pfad = pfad;
            DataContext = _imodel;
            _imodel.ShowImport1();

            _imodel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CurrentView")
                {
                    if (_imodel.CurrentView == null)
                    {
                        this.Close();
                    }
                }
            };

        }
    }
}
