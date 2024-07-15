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
    /// Interaktionslogik für CSVImport.xaml
    /// </summary>
    public partial class CSVImport : Window
    {
        private MainViewModel _mvmodel;
        public CSVImport()
        {
            _mvmodel = FindResource("mvmodel") as MainViewModel;
            InitializeComponent();
            Leiste.Benutzername = _mvmodel.Benutzer.Vorname + ", " + _mvmodel.Benutzer.Nachname;
        }
    }
}
