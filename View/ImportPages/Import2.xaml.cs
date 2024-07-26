using System.Reflection;
using System.Windows.Controls;

namespace RunTrack
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
            _imodel = FindResource("imodel") as ImportModel ?? new();
            DataContext = _imodel;

            _imodel.RundenArten = new(new LaufDBContext().RundenArten.ToList());

            btnBack.Click += (s, e) => _imodel.ShowImport1();
            btnWeiter.Click += (s, e) =>
            {
                _imodel.ShowImport3();
            };
            Load();
        }

        public void Load()
        {
            _imodel.KlasseItems = new();
            int klasseIndex = _imodel.Reihenfolge.IndexOf("Klasse");
            List<string> klassen = new();
            foreach (object item in _imodel.CSVListe)
            {
                string name = "Spalte" + (klasseIndex + 1).ToString();
                PropertyInfo? value = item.GetType().GetProperty(name) ?? null;

                if (value != null)
                {
                    string klasse = value.GetValue(item)?.ToString() ?? string.Empty;
                    if (klasse == null) continue;
                    if (!klassen.Contains(klasse))
                    {
                        klassen.Add(klasse);
                        _imodel.KlasseItems.Add(new KlasseItem { Bezeichnung = klasse });
                    }
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            if (comboBox == null) return;
            KlasseItem? klasseItem = comboBox.DataContext as KlasseItem;
            if (klasseItem == null) return;
            klasseItem.RundenArt = (RundenArt)comboBox.SelectedItem;
        }
    }
}
