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
    /// Interaktionslogik für CSVImport2.xaml
    /// </summary>
    public partial class CSVImport2 : Window
    {
        private string _pfad;
        private List<string> _reihenfolge;
        private string _schule;

        public CSVImport2(string pfad, List<string> reihenfolge, string schule, List<object> data)
        {
            this._pfad = pfad;
            this._reihenfolge = reihenfolge;
            this._schule = schule;
            InitializeComponent();

            btnCancel.Click += (s, e) => this.Close();
            btnWeiter.Click += (s, e) =>
            {
                MessageBox.Show("Weiter");
            };

        }
    }
}
