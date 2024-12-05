using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Import3.xaml
    /// </summary>
    public partial class Import3 : Page
    {
        private ImportModel _imodel;
        private MainModel _model;
        private List<string> _selectedFiles;

        public Import3()
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new();
            _model = FindResource("pmodel") as MainModel ?? new();
            DataContext = _imodel;
            _selectedFiles = new List<string>();

            btnSchliessen.Click += (s, e) =>
            {
                if (tbTitel.Text == "Fehler") _model.Navigate(_model.History[^1]);
                else
                {
                    object page = _model.History.FindLast(x => x.GetType() != typeof(Import1) && x.GetType() != typeof(Import2));
                    _model.Navigate(page);
                }
            };

            btnWeitereLaden.Click += (s, e) => ImportFiles();
        }

        private void BtnDateienAuswaehlen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFiles.Clear();
                _selectedFiles.AddRange(openFileDialog.FileNames);
                lbDateien.ItemsSource = _selectedFiles;
            }
        }

        private void ImportFiles()
        {
            try
            {
                foreach (var file in _selectedFiles)
                {
                    // Hier die Logik zum Importieren der Datei in die Datenbank einfügen
                    ImportIntoDB importIntoDB = new(_imodel);
                }

                tbTitel.Text = "Erfolg";
                tbMeldung.Text = "Import war erfolgreich.";
                tbTitel.Foreground = Brushes.Green;
                btnSchliessen.Content = "Schließen";
            }
            catch (ImportException ex)
            {
                tbTitel.Foreground = Brushes.Red;
                tbTitel.Text = "Fehler";
                tbMeldung.Text = ex.Message;
                btnSchliessen.Content = "Zurück";
            }
            catch (Exception)
            {
                tbTitel.Foreground = Brushes.Red;
                tbTitel.Text = "Fehler";
                tbMeldung.Text = "Ein unerwarteter Fehler ist aufgetreten.";
                btnSchliessen.Content = "Zurück";
            }
        }
    }
}
