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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Import3.xaml
    /// </summary>
    public partial class Import3 : Page
    {
        private ImportModel _imodel;
        public Import3()
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel;
            DataContext = _imodel;

            btnSchliessen.Click += (s, e) => _imodel.CurrentView = null;
            try
            {
                ImportIntoDB importIntoDB = new(_imodel);
                tbTitel.Text = "Erfolg";
                tbMeldung.Text = "Import war erfolgreich.";
                tbTitel.Foreground = Brushes.Green;
            }
            catch (ImportException ex)
            {
                tbMeldung.Text = ex.Message;
                tbTitel.Foreground = Brushes.Red;
            }
            catch (Exception)
            {
                tbMeldung.Text = "Ein unerwarteter Fehler ist aufgetreten.";
                tbTitel.Foreground = Brushes.Red;
            }
        }
    }
}
