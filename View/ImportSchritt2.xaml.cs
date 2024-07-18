using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
    public partial class ImportSchritt2 : Window
    {
        private ImportModel _imodel;

        public ImportSchritt2()
        {
            _imodel = FindResource("imodel") as ImportModel;
            InitializeComponent();
            DataContext = this;

            btnCancel.Click += (s, e) => this.Close();
            btnWeiter.Click += (s, e) =>
            {
                MessageBox.Show("Weiter");
            };

            int klasseIndex = _imodel.Reihenfolge.IndexOf("Klasse");
            List<string> klassen = new List<string>();
            foreach (object item in _imodel.CSVListe)
            {
                string name = "Spalte" + (klasseIndex + 1).ToString();
                PropertyInfo value = item.GetType().GetProperty(name);

                if (value != null)
                {
                    string klasse = value.GetValue(item).ToString();
                    if (!klassen.Contains(klasse))
                    {
                        klassen.Add(klasse);
                    }
                }
            }
        }
    }
}
