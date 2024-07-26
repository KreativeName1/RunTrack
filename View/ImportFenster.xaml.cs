using System.Windows;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für ImportFenster.xaml
    /// </summary>
    public partial class ImportFenster : Window
    {
        private ImportModel _imodel;

        public ImportFenster(string pfad)
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new ImportModel();
            _imodel.Pfad = pfad;
            DataContext = _imodel;
            _imodel.ShowImport1();

            // listen to CurrentView changes
            _imodel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CurrentView")
                {
                    if (_imodel.CurrentView == null)
                    {
                        Close();
                    }
                }
            };

        }
    }
}
