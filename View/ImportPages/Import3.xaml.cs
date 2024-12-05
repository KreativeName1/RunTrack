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
<<<<<<< HEAD
        public Import2 _import2;

        public Import3(Import2 ip2)
        {
            InitializeComponent();
            _import2 = ip2;
=======

        public Import3()
        {
            InitializeComponent();
>>>>>>> 480ab1fdf1e6a6d2526672c40836a237a955672b
            _imodel = FindResource("imodel") as ImportModel ?? new();
            _model = FindResource("pmodel") as MainModel ?? new();
            DataContext = _imodel;
            _selectedFiles = new List<string>();

<<<<<<< HEAD
            //btnSchliessen.Click += (s, e) =>
            //{
            //    if (tbTitel.Text == "Fehler") _model.Navigate(_model.History[^1]);
            //    else
            //    {
            //        object page = _model.History.FindLast(x => x.GetType() != typeof(Import1) && x.GetType() != typeof(Import2));
            //        _model.Navigate(page);
            //    }
            //};

            btnWeitereLaden.Click += (s, e) => {
                string[] lst = ip2._import1._verwaltung._tempSelectedFiles.ToArray();
                ip2._import1._verwaltung.PromptUserToSelectFile(lst);
                ip2._import1._verwaltung.UploadFiles_ClickPublic();
                ImportFiles(); };
=======
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
>>>>>>> 480ab1fdf1e6a6d2526672c40836a237a955672b
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
<<<<<<< HEAD
        }

        private void btnSchliessen_Click(object sender, RoutedEventArgs e)
        {
            if (tbTitel.Text == "Fehler") _model.Navigate(_model.History[^1]);
            else
            {
                object page = _model.History.FindLast(x => x.GetType() != typeof(Import1) && x.GetType() != typeof(Import2));
                _model.Navigate(page);
            }
=======
>>>>>>> 480ab1fdf1e6a6d2526672c40836a237a955672b
        }
    }
}
