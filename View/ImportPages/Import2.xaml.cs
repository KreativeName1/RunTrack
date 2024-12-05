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
        private MainModel _model;
        public Import1 _import1;

        public Import2(Import1 import1)
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new();
            _model = FindResource("pmodel") as MainModel ?? new();
            DataContext = _imodel;

            _imodel.RundenArten = new(new LaufDBContext().RundenArten.ToList());

            btnBack.Click += (s, e) =>
            {
                var lastPage = _model.History.FindLast(x => x.GetType() == typeof(Import1));
                if (lastPage != null)
                {
                    _model.Navigate(lastPage);
                }
            };
            btnWeiter.Click += (s, e) =>
            {
                if (_imodel.Pfad != null)
                {
<<<<<<< HEAD
                    _model.Navigate(new Import3(this));
=======
                    _model.Navigate(new Import3());
>>>>>>> 480ab1fdf1e6a6d2526672c40836a237a955672b
                }
            };
            Load();
            _import1 = import1;
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

        private void SetAllRundenArtenComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SetAllRundenArtenComboBox.SelectedItem is RundenArt selectedRundenArt)
            {
                if (_imodel?.KlasseItems != null)
                {
                    foreach (var klasseItem in _imodel.KlasseItems)
                    {
                        klasseItem.RundenArt = selectedRundenArt;
                    }
                }
                else
                {
                    // Handle the case where _imodel or KlasseItems is null
                    // Log an error or initialize KlasseItems if appropriate
                }
            }
        }

    }
}
