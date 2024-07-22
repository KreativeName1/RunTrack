using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Import2.xaml
    /// </summary>
    public partial class Import2 : Page
    {
        private ImportModel _imodel;

        public Import2()
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel;
            DataContext = _imodel;

            _imodel.RundenArten = new ObservableCollection<RundenArt>(new LaufDBContext().RundenArten.ToList());

            btnBack.Click += (s,e) => _imodel.ShowImport1();
            btnWeiter.Click += (s, e) =>
            {
                _imodel.ShowImport3();
            };
            Load();

            
        }

        public void Load() {
            _imodel.KlasseItems = new();
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
                        _imodel.KlasseItems.Add(new KlasseItem { Bezeichnung = klasse, RundenArt = _imodel.RundenArten[0] });
                    }
                }
            }
        }
    }
}
