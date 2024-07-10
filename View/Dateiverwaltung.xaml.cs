using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Klimalauf
{
    public partial class Dateiverwaltung : Window
    {
        private MainViewModel _mvmodel;

        public Dateiverwaltung()
        {
            InitializeComponent();
            _mvmodel = FindResource("mvmodel") as MainViewModel;
            ScannerName.Content = $"{_mvmodel.Benutzer.Vorname}, {_mvmodel.Benutzer.Nachname}";


        }


        private void LogoutIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _mvmodel.LstFiles = new ObservableCollection<FileItem>(FileItem.AlleLesen());
        }

        private void UploadFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "files (*.asv;*.db;*.csv)|*.asv;*.db;*.csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    string extension = Path.GetExtension(fileName).ToLower();

                    if (extension == ".asv" || extension == ".db" || extension == ".csv")
                    {
                        // Datei in die Liste hinzufügen
                        _mvmodel.LstFiles.Add(new FileItem
                        {
                            FileName = Path.GetFileName(fileName),
                            UploadDate = DateTime.Now
                        });

                        // Datei in den Dateien-Ordner kopieren
                        Directory.CreateDirectory("Dateien");
                        string destPath = Path.Combine("Dateien", Path.GetFileName(fileName));
                        File.Copy(fileName, destPath, true);
                    }
                    else
                    {
                        MessageBox.Show($"Die Datei {fileName} hat eine ungültige Erweiterung und kann nicht hochgeladen werden.");
                    }
                }
            }
        }

        private void DeleteSelectedFiles_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _mvmodel.LstFiles.Count; i++)
            {
                if (_mvmodel.LstFiles[i].IsSelected)
                {
                Trace.WriteLine(_mvmodel.LstFiles[i].FileName);
                    MessageBoxResult result = MessageBox.Show($"Willst du die Datei '{_mvmodel.LstFiles[i].FileName}' wirklich löschen?", "Datei Löschen", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        File.Delete(Path.Combine("Dateien", _mvmodel.LstFiles[i].FileName));
                        _mvmodel.LstFiles.RemoveAt(i);
                    }
                }
            }
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool newValue = (SelectAllCheckBox.IsChecked == true);
            foreach (FileItem file in _mvmodel.LstFiles)
            {
                file.IsSelected = newValue;
            }

            SelectAllTextBlock.Text = SelectAllCheckBox.IsChecked == true ? "Deselect All" : "Select All";
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Scanner adminPanel = new Scanner();
            adminPanel.Show();
            this.Close();
        }

        private void DownloadFiles_Click(object sender, RoutedEventArgs e)
        {
            // get the selected file
            foreach (FileItem file in _mvmodel.LstFiles)
            {
                if (file.IsSelected)
                {
                    string sourcePath = Path.Combine("Dateien", file.FileName);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        FileName = file.FileName,
                        Filter = "files (*.asv;*.db;*.csv)|*.asv;*.db;*.csv"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.Copy(sourcePath, saveFileDialog.FileName, true);
                    }
                }
            }
        }
    }
}
