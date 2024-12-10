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

            files.IsTabStop = false;
            btnViewer.IsTabStop = false;
            btnExport.IsTabStop = false;
            btnLoeschen.IsTabStop = false;
            btnSchliessen.IsTabStop = false;
            FilesListBox.IsTabStop = false;
            btnDBView.IsTabStop = false;
            btnUpload.IsTabStop = false;
        }

        // Methode zum Hochladen von Dateien
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

                // Überprüfen, ob mehr als 10 Dateien ausgewählt wurden
                if (_tempSelectedFiles.Count > 10)
                {
                    new Popup().Display("Fehler", "Es können maximal 10 Dateien gleichzeitig hochgeladen werden.", PopupType.Warning, PopupButtons.Ok);
                    return;
                }

                if (_tempSelectedFiles.Count > 1)
                {
                    new Popup().Display("Error", "Es kann derzeit nur eine Datei gleichzeitig ausgewählt werden.\n\nEine Mehrfachauswahl ist momentan nicht möglich.", PopupType.Error, PopupButtons.Ok);
                    return;
                    //// Wenn mehrere Dateien ausgewählt wurden, Benutzer zur Auswahl auffordern
                    //string selectedFile = PromptUserToSelectFile(_tempSelectedFiles.ToArray());
                    //if (!string.IsNullOrEmpty(selectedFile))
                    //{
                    //    checkFile(selectedFile);
                    //}
                }
                else
                {
                    // Nur eine Datei ausgewählt
                    checkFile(_tempSelectedFiles[0]);
                }
            }
        }

        // Methode zur Auswahl einer Datei aus mehreren
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

        // Überprüft die Datei und führt ggf. die Upload-Operation durch
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

        // Zeigt verbleibende Dateien zum Importieren an
        public void ShowRemainingFiles()
        {
            var remainingFilesstrings = _tempSelectedFiles;

            if (remainingFilesstrings.Count == 0)
            {
                new Popup().Display("Information", "Es sind keine weiteren Dateien zum Importieren vorhanden.", PopupType.Info, PopupButtons.Ok);
                return;
            }

            List<FileListItem> fileItems = remainingFilesstrings.Select(fileItem =>
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
            ; // Liste für die verbleibenden Dateien für den Import

            SelectFileWindow selectFileWindow = new SelectFileWindow(fileItems); // neues Fenster öffnen, um weiter Dateien auswählen zu können

            try
            {

                selectFileWindow.ShowDialog(); // Dialog anzeigen
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (File.Exists(selectFileWindow.SelectedFile))
            {
                checkFile(selectFileWindow.SelectedFile); // Datei überprüfen
            }
            else
            {
                new Popup().Display("Fehler", $"Die ausgewählte Datei existiert nicht mehr.", PopupType.Error, PopupButtons.Ok); // Fehler ausgeben
            }

        }

        // Löscht ausgewählte Dateien
        private void DeleteSelectedFiles_Click(object sender, RoutedEventArgs e)
        {
            string extra = string.Empty;
            bool logout = false;
            List<string> otherFilesToDelete = new List<string>();

            // Überprüfen, ob die Datenbankdatei gelöscht werden soll
            for (int i = 0; i < _dvmodel.LstFiles.Count; i++)
            {
                if (_dvmodel.LstFiles[i].IsSelected)
                {
                    string fileName = _dvmodel.LstFiles[i].FileName;

                    // Wenn es sich um die Datenbank handelt
                    if (Path.GetFileName(fileName) == "EigeneDatenbank.db")
                    {
                        extra = "(Die Datenbank des Programms wird gelöscht und alle bisherigen Daten gehen verloren!)";
                        logout = true;

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

                                    // Logout nach Datenbanklöschung
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
                        // Andere Dateien zur Löschung hinzufügen
                        otherFilesToDelete.Add(fileName);
                    }
                }
            }

            // Löschen anderer Dateien
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
                            // Entfernen der Datei aus der Liste
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

        // Klick auf die "Select All" Checkbox
        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool newValue = (SelectAllCheckBox.IsChecked == true);  // Bestimmen, ob alle Dateien ausgewählt oder abgewählt werden sollen
            foreach (FileItem file in _dvmodel.LstFiles) file.IsSelected = newValue;  // Alle Dateien basierend auf der Auswahl aktualisieren

            // Text der SelectAllTextBlock aktualisieren (Wird zu "Deselect All" oder "Select All" je nach Auswahl)
            SelectAllTextBlock.Text = SelectAllCheckBox.IsChecked == true ? "Deselect All" : "Select All";
        }

        // Klick auf den "Close" Button (Fenster schließen)
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Object? page = _pmodel.History.FindLast(x => !new[] { typeof(ImportUbersicht), typeof(Import1), typeof(Import2), typeof(Import3), GetType() }.Contains(x.GetType()));
            if (page != null) _pmodel.Navigate(page);  // Navigiere zur vorherigen Seite, wenn sie existiert
        }

        // Klick auf den "Download" Button
        private void DownloadFiles_Click(object sender, RoutedEventArgs e)
        {
            foreach (FileItem file in _dvmodel.LstFiles)
            {
                if (file.IsSelected)  // Wenn die Datei ausgewählt wurde
                {
                    string sourcePath = Path.Combine("Dateien", file.FileName ?? string.Empty);  // Pfad zur Datei
                    SaveFileDialog saveFileDialog = new()  // Dialog zum Speichern der Datei
                    {
                        FileName = file.FileName,
                        Filter = "files (*.asv;*.db;*.csv)|*.asv;*.db;*.csv"  // Filter für Dateiformate
                    };

                    if (saveFileDialog.ShowDialog() == true)  // Zeige Dialog und speichere die Datei
                    {
                        File.Copy(sourcePath, saveFileDialog.FileName ?? string.Empty, true);
                    }
                }
            }
        }

        // Variablen für Sortierreihenfolge
        private bool sortByFileNameAscending = true;
        private bool sortByUploadDateAscending = true;

        // Klick auf den "FileName" Label (für Sortierung nach Dateiname)
        private void FileNameLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SortFilesByPropertyName("FileName", ref sortByFileNameAscending);
        }

        // Klick auf den "UploadDate" Label (für Sortierung nach Uploaddatum)
        private void UploadDateLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SortFilesByPropertyName("UploadDate", ref sortByUploadDateAscending);
        }

        // Methode zum Sortieren der Dateien nach einem bestimmten Property
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
            foreach (var item in sortedList) _dvmodel.LstFiles.Add(item);  // Sortierte Liste zurücksetzen
        }

        // MouseEnter und MouseLeave Events für die Labels
        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock label) label.Foreground = Brushes.Green;  // Textfarbe ändern, wenn die Maus über dem Label ist
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock label) label.Foreground = Brushes.Black;  // Zurücksetzen der Textfarbe, wenn die Maus das Label verlässt
        }

        // Drag & Drop Event für das Hinzufügen von Dateien
        private void FilesListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> files = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));  // Ausgewählte Dateien
                _dvmodel.IsDragging = false;
                _tempSelectedFiles.Clear();
                _tempSelectedFiles.AddRange(files);

                // Überprüfen, ob mehr als 10 Dateien hinzugefügt wurden
                if (_tempSelectedFiles.Count > 10)
                {
                    new Popup().Display("Fehler", "Es können maximal 10 Dateien gleichzeitig hochgeladen werden.", PopupType.Warning, PopupButtons.Ok);
                    return;
                }

                if (_tempSelectedFiles.Count > 1)
                {
                    new Popup().Display("Error", "Es kann derzeit nur eine Datei gleichzeitig ausgewählt werden.\n\nEine Mehrfachauswahl ist momentan nicht möglich.", PopupType.Error, PopupButtons.Ok);
                    return;
                    //// Wenn mehrere Dateien, den Benutzer zur Auswahl auffordern
                    //string selectedFile = PromptUserToSelectFile(_tempSelectedFiles.ToArray());
                    //if (!string.IsNullOrEmpty(selectedFile))
                    //{
                    //    checkFile(selectedFile);
                    //}
                }
                else
                {
                    // Wenn nur eine Datei, diese direkt verarbeiten
                    checkFile(_tempSelectedFiles[0]);
                }
            }
        }

        // Änderung der Auswahl in der ListBox
        private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateExportButtonVisibility();  // Sichtbarkeit des Export-Buttons aktualisieren

            foreach (var item in e.AddedItems.OfType<FileItem>())
            {
                item.IsSelected = true;  // Checkbox aktivieren
            }

            foreach (var item in e.RemovedItems.OfType<FileItem>())
            {
                item.IsSelected = false;  // Checkbox deaktivieren
            }
        }

        // MouseDown Event für das Entfernen von Dateien aus der Auswahl
        private void FilesListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element &&
                element.DataContext is FileItem clickedItem &&
                FilesListBox.SelectedItems.Contains(clickedItem))
            {
                clickedItem.IsSelected = false;  // Auswahl entfernen
                FilesListBox.SelectedItems.Remove(clickedItem);  // Datei aus der Auswahl entfernen

                UpdateExportButtonVisibility();  // Sichtbarkeit des Export-Buttons aktualisieren
                e.Handled = true;  // Event als verarbeitet markieren
            }
        }

        // Export-Button Sichtbarkeit basierend auf der Auswahl
        private void UpdateExportButtonVisibility()
        {
            btnExport.Visibility = FilesListBox.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Klick auf den "Open Folder" Button
        private void files_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderPath = Path.Combine(currentDirectory, "Dateien");

            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer.exe", folderPath);  // Ordner im Explorer öffnen
            }
            else
            {
                MessageBox.Show("Der Ordner 'Dateien' existiert nicht.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Klick auf den "DB View" Button (Datenbank-Datei öffnen)
        private void btnDBView_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = "files (*.db)|*.db"  // Filter für Datenbankdateien
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string dbPath = openFileDialog.FileName;
                if (File.Exists(dbPath))
                {
                    _pmodel.Navigate(new Datenuebersicht());  // Zur Datenübersicht navigieren
                    DatenuebersichtModel model = FindResource("dumodel") as DatenuebersichtModel ?? new();
                    model.ConnectionString = dbPath;
                    model.ReadOnly = true;
                }
                else
                {
                    new Popup().Display("Fehler", "Die ausgewählte Datei existiert nicht mehr.", PopupType.Error, PopupButtons.Ok);
                }
            }
        }

        // Klick auf den "Viewer" Button (Datei anzeigen)
        private void btnViewer_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = _dvmodel.LstFiles.FirstOrDefault(f => f.IsSelected)?.FileName;
            if (selectedFile == null)
            {
                new Popup().Display("Fehler", "Bitte wählen Sie eine Datei aus.", PopupType.Error, PopupButtons.Ok);
                return;
            }

            selectedFile = Path.Combine("Dateien", selectedFile);
            _pmodel.Navigate(new Datenuebersicht());  // Zur Datenübersicht navigieren
            DatenuebersichtModel model = FindResource("dumodel") as DatenuebersichtModel ?? new();
            model.ConnectionString = selectedFile;  // Pfad der ausgewählten Datei setzen
            model.ReadOnly = true;
        }

        private void SelectAllCheckBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectAllCheckBox.IsChecked = !SelectAllCheckBox.IsChecked;
                SelectAllCheckBox_Click(sender, new RoutedEventArgs());
            }
        }
    }
}