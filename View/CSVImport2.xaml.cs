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

            int klasseIndex = _reihenfolge.IndexOf("Klasse");
            List<string> klassen = new List<string>();
            foreach (object item in data)
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
            //DataGridComboBoxColumn dataGridComboBoxColumn = new();
            //dataGridComboBoxColumn.Header = "Rundenart";
            //dataGridComboBoxColumn.ItemsSource = new List<string> { "Zeit", "Runden" };
            //dataGridComboBoxColumn.SelectedItemBinding = new Binding("RundenArt") { Mode = BindingMode.TwoWay };

            //DataGridTextColumn dgtKlassen = new DataGridTextColumn();

            //dgtKlassen.Header = "Klasse";
            //dgtKlassen.Binding = new Binding("K") { Mode = BindingMode.OneWay };
            //dgtKlassen.IsReadOnly = true;

            //dgKlassen.Columns.Add(dgtKlassen);
            //dgKlassen.Columns.Add(dataGridComboBoxColumn);

            //foreach (string klasse in klassen)
            //{
            //    // add a klasse and a rundenart combobox
            //    dgKlassen.Items.Add(new { K = klasse, R = "" });
            //}
        }
    }
}
