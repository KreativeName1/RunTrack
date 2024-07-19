using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            _imodel = FindResource("imodel") as ImportModel;
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
