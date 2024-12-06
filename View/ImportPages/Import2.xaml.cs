using System.Reflection;
using System.Windows;
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

        public Import2()
        {
            InitializeComponent();
            // Initialisiere ImportModel und MainModel
            _imodel = FindResource("imodel") as ImportModel ?? new();
            _model = FindResource("pmodel") as MainModel ?? new();
            DataContext = _imodel;

            // Verstecke das Lade-Overlay beim Laden der Seite
            this.Loaded += (s, e) =>
            {
                LoadOverlay.Visibility = Visibility.Hidden;
            };

            // Lade die RundenArten aus der Datenbank
            _imodel.RundenArten = new(new LaufDBContext().RundenArten.ToList());

            // Event-Handler für den Zurück-Button
            btnBack.Click += (s, e) => _model.Navigate(_model.History.FindLast(x => x.GetType() == typeof(Import1)));

            // Event-Handler für den Weiter-Button
            btnWeiter.Click += (s, e) =>
            {
                LoadOverlay.Visibility = Visibility.Visible;

                // Starte einen neuen Task, um die Navigation zu verzögern
                Task.Run(() =>
                {
                    Thread.Sleep(500);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _model.Navigate(new ImportUbersicht());
                    });
                });
                return;
            };
            Load();
        }

        // Methode zum Laden der Klassen-Items
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

        // Event-Handler für die Auswahländerung in der ComboBox
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            if (comboBox == null) return;
            KlasseItem? klasseItem = comboBox.DataContext as KlasseItem;
            if (klasseItem == null) return;
            klasseItem.RundenArt = (RundenArt)comboBox.SelectedItem;
        }

        // Event-Handler für die Auswahländerung in der SetAllRundenArtenComboBox
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
                    // Behandle den Fall, wenn _imodel oder KlasseItems null ist
                    // Logge einen Fehler oder initialisiere KlasseItems, falls angemessen
                }
            }
        }
    }
}
