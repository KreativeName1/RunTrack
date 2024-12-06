using Microsoft.Win32;
using RunTrack.Model;
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
        private List<string> _tempSelectedFiles = new List<string>();

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
                _tempSelectedFiles.Clear();
                _tempSelectedFiles.AddRange(openFileDialog.FileNames);

                // Überprüfen, ob die Gesamtanzahl der Dateien die Grenze von 10 überschreitet
                if (_tempSelectedFiles.Count > 10)
                {
                    new Popup().Display("Fehler", "Es können maximal 10 Dateien gleichzeitig hochgeladen werden.", PopupType.Warning, PopupButtons.Ok);
                    return;
                }

                if (_tempSelectedFiles.Count > 1)
                {
                    // Mehrere Dateien ausgewählt, Benutzer zur Auswahl einer Datei auffordern
                    string selectedFile = PromptUserToSelectFile(_tempSelectedFiles.ToArray());
                    if (!string.IsNullOrEmpty(selectedFile))
                    {
                        checkFile(selectedFile);
                    }
                }
                else
                {
                    // Nur eine Datei ausgewählt
                    checkFile(_tempSelectedFiles[0]);
                }
            }
        }

        private string PromptUserToSelectFile(string[] fileNames)
        {
            var fileItems = fileNames.Select(fileName =>
            {
                string displayPath = Path.GetFullPath(fileName);
                string destPath = Path.Combine("Dateien", Path.GetFileName(fileName));
                return new FileListItem
                {
                    Pfad = displayPath,
                    Name = Path.GetFileName(fileName),
                    InfoVisible = File.Exists(destPath),
                    Tooltip = File.Exists(destPath) ? $"Die Datei {Path.GetFileName(fileName)} existiert bereits." : string.Empty
                };
            }).ToList();

            var selectFileWindow = new SelectFileWindow(fileItems);
            if (selectFileWindow.ShowDialog() == true)
            {
                if (File.Exists(selectFileWindow.SelectedFile))
                {
                    return selectFileWindow.SelectedFile;
                }
                new Popup().Display("Fehler", $"Die ausgewählte Datei existiert nicht mehr.", PopupType.Error, PopupButtons.Ok);
            }
            return string.Empty;
        }



        private void checkFile(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            if (extension != ".asv" && extension != ".db" && extension != ".csv")
            {
                new Popup().Display("Fehler", $"Die Datei {fileName} hat eine ungültige Erweiterung und kann nicht hochgeladen werden.", PopupType.Error, PopupButtons.Ok);
                return;
            }

            string destPath = Path.Combine("Dateien", Path.GetFileName(fileName));

            // Falls Datei bereits existiert
            if (File.Exists(destPath))
            {
                string extra;
                if (Path.GetFileName(fileName) == "EigeneDatenbank.db") extra = "(Die Datenbank des Programms wird überschrieben und alle bisherigen Daten gehen verloren!)";
                else extra = string.Empty;

                bool? result = new Popup().Display("Datei überschreiben", $"Die Datei '{Path.GetFileName(fileName)}' existiert bereits. Willst du sie überschreiben? {extra}", PopupType.Question, PopupButtons.YesNo);

                if (result == false)
                {
                    if (_tempSelectedFiles.Count > 1)
                    {
                        // Benutzer zur Auswahl einer Datei auffordern
                        string selectedFile = PromptUserToSelectFile(_tempSelectedFiles.ToArray());
                        if (!string.IsNullOrEmpty(selectedFile))
                        {
                            checkFile(selectedFile);
                        }
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // Datei in die Liste hinzufügen
            _dvmodel.LstFiles.Add(new FileItem
            {
                FileName = Path.GetFileName(fileName),
                UploadDate = DateTime.Now
            });

            // Datei in den Dateien-Ordner kopieren
            Directory.CreateDirectory("Dateien");
            File.Copy(fileName, destPath, true);

            if (extension == ".asv" || extension == ".csv")
            {
                _pmodel.Navigate(new Import1(Path.GetFullPath(destPath)));

                _dvmodel.LstFiles = new ObservableCollection<FileItem>(FileItem.AlleLesen());
            }
        }

        public void ShowRemainingFiles()
        {
            var remainingFilesstrings = _tempSelectedFiles;

            if (remainingFilesstrings.Count == 0)
            {
                new Popup().Display("Information", "Es sind keine weiteren Dateien zum Importieren vorhanden.", PopupType.Info, PopupButtons.Ok);
                return;
            }

            var fileItems = remainingFilesstrings.Select(fileItem =>
            {
                string displayPath = Path.GetFullPath(fileItem);
                return new FileListItem
                {
                    Pfad = fileItem,
                    Name = Path.GetFileName(fileItem),
                    InfoVisible = File.Exists(fileItem),
                    Tooltip = File.Exists(fileItem) ? $"Die Datei {Path.GetFileName(fileItem)} existiert bereits." : string.Empty
                };
            }).ToList();
;

            var selectFileWindow = new SelectFileWindow(fileItems);
            if (selectFileWindow.ShowDialog() == true)
            {
                if (File.Exists(selectFileWindow.SelectedFile))
                {
                    checkFile(selectFileWindow.SelectedFile);
                }
                else
                {
                    new Popup().Display("Fehler", $"Die ausgewählte Datei existiert nicht mehr.", PopupType.Error, PopupButtons.Ok);
                }
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
                            $"Willst du die Datei '{fileName}' wirklich löschen?\n{extra}",
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
                string fileList = string.Join(Environment.NewLine, otherFilesToDelete.Select(f => $"• {f}"));
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
                _tempSelectedFiles.Clear();
                _tempSelectedFiles.AddRange(files);

                // Überprüfen, ob die Gesamtanzahl der Dateien die Grenze von 10 überschreitet
                if (_tempSelectedFiles.Count > 10)
                {
                    new Popup().Display("Fehler", "Es können maximal 10 Dateien gleichzeitig hochgeladen werden.", PopupType.Warning, PopupButtons.Ok);
                    return;
                }

                if (_tempSelectedFiles.Count > 1)
                {
                    // Mehrere Dateien ausgewählt, Benutzer zur Auswahl einer Datei auffordern
                    string selectedFile = PromptUserToSelectFile(_tempSelectedFiles.ToArray());
                    if (!string.IsNullOrEmpty(selectedFile))
                    {
                        checkFile(selectedFile);
                    }
                }
                else
                {
                    // Nur eine Datei ausgewählt
                    checkFile(_tempSelectedFiles[0]);
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
