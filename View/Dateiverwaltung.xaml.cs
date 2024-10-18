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
        private DateiVerwaltungModel _dvmodel;
        private MainModel _pmodel;

        public Dateiverwaltung()
        {
            InitializeComponent();
            _dvmodel = FindResource("dvmodel") as DateiVerwaltungModel ?? new();
            _pmodel = FindResource("pmodel") as MainModel ?? new();
            _dvmodel.LstFiles = new(FileItem.AlleLesen());

            FilesListBox.DragEnter += (s, e) => _dvmodel.IsDragging = true;
            DragDropOverlay.DragLeave += (s, e) => _dvmodel.IsDragging = false;

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
                    checkFile(fileName);
                }
            }
        }


        private void checkFile(string fileName)
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
                    _pmodel.Navigate(new Import1(Path.GetFullPath(destPath)));

                    _dvmodel.LstFiles = new ObservableCollection<FileItem>(FileItem.AlleLesen());
                }
            }
            else
            {
                MessageBox.Show($"Die Datei {fileName} hat eine ungültige Erweiterung und kann nicht hochgeladen werden.");
            }
        }
        private void DeleteSelectedFiles_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _dvmodel.LstFiles.Count; i++)
            {
                if (_dvmodel.LstFiles[i].IsSelected)
                {
                    bool? result = new Popup().Display($"Datei löschen", $"Willst du die Datei '{_dvmodel.LstFiles[i].FileName}' wirklich löschen?", PopupType.Question, PopupButtons.YesNo);
                    if (result == true)
                    {
                        if (_dvmodel.LstFiles[i].FileName == null) continue;
                        File.Delete(Path.Combine("Dateien", _dvmodel.LstFiles[i].FileName ?? string.Empty));
                        _dvmodel.LstFiles.RemoveAt(i);
                    }
                }
            }
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool newValue = (SelectAllCheckBox.IsChecked == true);
            foreach (FileItem file in _dvmodel.LstFiles) file.IsSelected = newValue;

            SelectAllTextBlock.Text = SelectAllCheckBox.IsChecked == true ? "Deselect All" : "Select All";
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Object? page = _pmodel.History.FindLast(x => !new[] { typeof(Import1), typeof(Import2), typeof(Import3), GetType() }.Contains(x.GetType()));
            if (page != null) _pmodel.Navigate(page);
        }

        private void DownloadFiles_Click(object sender, RoutedEventArgs e)
        {
            foreach (FileItem file in _dvmodel.LstFiles)
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
                sortedList = ascending ? _dvmodel.LstFiles.OrderBy(f => f.FileName).ToList()
                                                             : _dvmodel.LstFiles.OrderByDescending(f => f.FileName).ToList();
                sortByFileNameAscending = !ascending;
            }
            else if (propertyName == "UploadDate")
            {
                sortedList = ascending ? _dvmodel.LstFiles.OrderBy(f => f.UploadDate).ToList()
                                                             : _dvmodel.LstFiles.OrderByDescending(f => f.UploadDate).ToList();
                sortByUploadDateAscending = !ascending;
            }
            else return;

            _dvmodel.LstFiles.Clear();
            foreach (var item in sortedList) _dvmodel.LstFiles.Add(item);
        }


        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock label) label.Foreground = Brushes.Green;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock label) label.Foreground = Brushes.Black;
        }

        private void FilesListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> files = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
                _dvmodel.IsDragging = false;
                files.RemoveAll(x => !x.Contains(".db") && !x.Contains(".sqlite") && !x.Contains(".csv"));

                string ausgabe = string.Empty;
                foreach (string file in files)
                {
                    checkFile(file);
                }
            }
        }
    }
}
