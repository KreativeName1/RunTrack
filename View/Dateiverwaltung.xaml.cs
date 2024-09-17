using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RunTrack
{
    public partial class Dateiverwaltung : Page
    {
        private MainViewModel _dvmodel;
        private PageModel _pmodel;

        public Dateiverwaltung()
        {
            InitializeComponent();
            _dvmodel = FindResource("dvmodel") as MainViewModel ?? new();
            _pmodel = FindResource("pmodel") as PageModel ?? new();
            _dvmodel.LstFiles = new(FileItem.AlleLesen());
        }

        private void UploadFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
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
                        _dvmodel.LstFiles.Add(new FileItem
                        {
                            FileName = Path.GetFileName(fileName),
                            UploadDate = DateTime.Now
                        });

                        // Datei in den Dateien-Ordner kopieren
                        Directory.CreateDirectory("Dateien");
                        string destPath = Path.Combine("Dateien", Path.GetFileName(fileName));
                        File.Copy(fileName, destPath, true);

                        if (extension == ".asv" || extension == ".csv")
                        {
                            ImportFenster fenster = new(Path.GetFullPath(destPath));
                            fenster.ShowDialog();
                            _mvmodel.LstFiles = new ObservableCollection<FileItem>(FileItem.AlleLesen());
                        }
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
                    MessageBoxResult result = MessageBox.Show($"Willst du die Datei '{_mvmodel.LstFiles[i].FileName}' wirklich löschen?", "Datei Löschen", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (_mvmodel.LstFiles[i].FileName == null) continue;
                        File.Delete(Path.Combine("Dateien", _mvmodel.LstFiles[i].FileName ?? string.Empty));
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
            Object? page  = _pmodel.History.FindLast(p => p.GetType() != GetType());
            if (page != null) _pmodel.Navigate(page);
        }

        private void DownloadFiles_Click(object sender, RoutedEventArgs e)
        {
            // get the selected file
            foreach (FileItem file in _mvmodel.LstFiles)
            {
                if (file.IsSelected)
                {
                    string sourcePath = Path.Combine("Dateien", file.FileName ?? string.Empty);
                    SaveFileDialog saveFileDialog = new()
                    {
                        FileName = file.FileName,
                        Filter = "files (*.asv;*.db;*.csv)|*.asv;*.db;*.csv"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.Copy(sourcePath, saveFileDialog.FileName ?? string.Empty, true);
                    }
                }
            }
        }

        private bool sortByFileNameAscending = true;
        private bool sortByUploadDateAscending = true;

        private void FileNameLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SortFilesByPropertyName("FileName", ref sortByFileNameAscending);
        }

        private void UploadDateLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SortFilesByPropertyName("UploadDate", ref sortByUploadDateAscending);
        }

        private void SortFilesByPropertyName(string propertyName, ref bool ascending)
        {
            List<FileItem> sortedList;
            if (propertyName == "FileName")
            {
                sortedList = ascending ? _mvmodel.LstFiles.OrderBy(f => f.FileName).ToList()
                                       : _mvmodel.LstFiles.OrderByDescending(f => f.FileName).ToList();
                sortByFileNameAscending = !ascending;
            }
            else if (propertyName == "UploadDate")
            {
                sortedList = ascending ? _mvmodel.LstFiles.OrderBy(f => f.UploadDate).ToList()
                                       : _mvmodel.LstFiles.OrderByDescending(f => f.UploadDate).ToList();
                sortByUploadDateAscending = !ascending;
            }
            else
            {
                return;
            }

            _mvmodel.LstFiles.Clear();
            foreach (var item in sortedList)
            {
                _mvmodel.LstFiles.Add(item);
            }
        }


        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock label)
            {
                label.Foreground = Brushes.Green; // Ändere die Textfarbe oder andere Eigenschaften für die Hervorhebung
            }
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock label)
            {
                label.Foreground = Brushes.Black; // Setze die Textfarbe oder andere Eigenschaften zurück
            }
        }

    }
}
