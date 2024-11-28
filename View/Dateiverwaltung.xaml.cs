using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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

            if (extension != ".asv" && extension != ".db" && extension != ".csv")
            {
                new Popup().Display("Fehler", $"Die Datei {fileName} hat eine ungültige Erweiterung und kann nicht hochgeladen werden.", PopupType.Error, PopupButtons.Ok);
                return;
            }

            // Falls Datei bereits existiert
            if (File.Exists(Path.Combine("Dateien", Path.GetFileName(fileName))))
            {
                string extra;
                if (Path.GetFileName(fileName) == "EigeneDatenbank.db") extra = "(Die Datenbank des Programms wird überschrieben und alle bisherigen Daten gehen verloren!)";
                else extra = string.Empty;

                bool? result = new Popup().Display("Datei überschreiben", $"Die Datei '{Path.GetFileName(fileName)}' existiert bereits. Willst du sie überschreiben? {extra}", PopupType.Question, PopupButtons.YesNo);
                if (result == false) return;
            }

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
        private void DeleteSelectedFiles_Click(object sender, RoutedEventArgs e)
        {
            string extra = string.Empty;
            bool logout = false;
            List<string> otherFilesToDelete = new List<string>();

            // First, check for the database file and handle it
            for (int i = 0; i < _dvmodel.LstFiles.Count; i++)
            {
                if (_dvmodel.LstFiles[i].IsSelected)
                {
                    string fileName = _dvmodel.LstFiles[i].FileName;

                    // Check if it's the database file
                    if (Path.GetFileName(fileName) == "EigeneDatenbank.db")
                    {
                        extra = "(Die Datenbank des Programms wird gelöscht und alle bisherigen Daten gehen verloren!)";
                        logout = true;

                        // Ask for confirmation for database file
                        bool? result = new Popup().Display($"Datei löschen",
                            $"Willst du die Datei '{fileName}' wirklich löschen? {extra}",
                            PopupType.Question,
                            PopupButtons.YesNo);
                        extra = string.Empty;

                        if (result == true)
                        {
                            if (fileName != null)
                            {
                                try
                                {
                                    File.Delete(Path.Combine("Dateien", fileName));
                                    _dvmodel.LstFiles.RemoveAt(i);

                                    // Logout and navigate to MainWindow if database was deleted
                                    if (logout)
                                    {
                                        _pmodel.Benutzer = null;
                                        _pmodel.Navigate(new MainWindow());
                                    }
                                }
                                catch (IOException ex)
                                {
                                    new Popup().Display("Fehler",
                                        $"Die Datei '{fileName}' konnte nicht gelöscht werden. {ex.Message}",
                                        PopupType.Error,
                                        PopupButtons.Ok);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Add other files to the list to be deleted
                        otherFilesToDelete.Add(fileName);
                    }
                }
            }

            // If there are other files to delete, show them in a second popup
            if (otherFilesToDelete.Count > 0)
            {
                string fileList = string.Join(Environment.NewLine, otherFilesToDelete);
                bool? result = new Popup().Display($"Dateien löschen",
                    $"Möchtest du folgende Dateien löschen?{Environment.NewLine}{fileList}",
                    PopupType.Question,
                    PopupButtons.YesNo);

                if (result == true)
                {
                    foreach (var file in otherFilesToDelete)
                    {
                        try
                        {
                            File.Delete(Path.Combine("Dateien", file));
                            // Remove the file from the list after deletion
                            // After deleting other files, loop through and remove them from the ObservableCollection
                            foreach (var fileToDelete in otherFilesToDelete)
                            {
                                var fileItem = _dvmodel.LstFiles.FirstOrDefault(f => f.FileName == fileToDelete);
                                if (fileItem != null)
                                {
                                    _dvmodel.LstFiles.Remove(fileItem);
                                }
                            }

                        }
                        catch (IOException ex)
                        {
                            new Popup().Display("Fehler",
                                $"Die Datei '{file}' konnte nicht gelöscht werden. {ex.Message}",
                                PopupType.Error,
                                PopupButtons.Ok);
                        }
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

        private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Sichtbarkeit des Buttons aktualisieren, basierend auf der aktuellen Auswahl
            UpdateExportButtonVisibility();

            // Verarbeite hinzugefügte Elemente
            foreach (var item in e.AddedItems.OfType<FileItem>())
            {
                item.IsSelected = true; // Checkbox aktivieren
            }

            // Verarbeite entfernte Elemente
            foreach (var item in e.RemovedItems.OfType<FileItem>())
            {
                item.IsSelected = false; // Checkbox deaktivieren
            }
        }

        private void FilesListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Bestimmen, ob ein FileItem angeklickt wurde
            if (e.OriginalSource is FrameworkElement element &&
                element.DataContext is FileItem clickedItem &&
                FilesListBox.SelectedItems.Contains(clickedItem))
            {
                // Wenn das Element bereits ausgewählt ist, Auswahl entfernen
                clickedItem.IsSelected = false;
                FilesListBox.SelectedItems.Remove(clickedItem);

                // Sichtbarkeit des Buttons aktualisieren
                UpdateExportButtonVisibility();

                // Ereignis als verarbeitet markieren
                e.Handled = true;
            }
        }

        private void UpdateExportButtonVisibility()
        {
            // Button nur anzeigen, wenn mindestens ein Element ausgewählt ist
            btnExport.Visibility = FilesListBox.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }


        private void files_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string folderPath = Path.Combine(currentDirectory, "Dateien");

            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer.exe", folderPath);
            }
            else
            {
                MessageBox.Show("Der Ordner 'Dateien' existiert nicht.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
