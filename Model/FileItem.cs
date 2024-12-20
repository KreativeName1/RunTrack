using System.ComponentModel;
using System.IO;

namespace RunTrack
{
    // Definiert die Klasse FileItem, die das Interface INotifyPropertyChanged implementiert
    public class FileItem : INotifyPropertyChanged
    {
        // Private Variable, die den Ausgewählt-Status speichert
        private bool _isSelected = false;

        // Öffentliche Eigenschaften für Dateiname und Upload-Datum
        public string? FileName { get; set; }
        public DateTime? UploadDate { get; set; }

        // Öffentliche Eigenschaft für den Ausgewählt-Status mit Benachrichtigung bei Änderung
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        // Event, das ausgelöst wird, wenn sich eine Eigenschaft ändert
        public event PropertyChangedEventHandler? PropertyChanged;

        // Methode, die das PropertyChanged-Event auslöst
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Standardkonstruktor
        public FileItem() { }

        // Konstruktor, der Dateiname und Upload-Datum initialisiert
        public FileItem(string fileName, DateTime uploadDate)
        {
            FileName = fileName;
            UploadDate = uploadDate;
        }

        // Statische Methode, die alle Dateien im Verzeichnis "Dateien" liest und als Liste von FileItem zurückgibt
        public static List<FileItem> AlleLesen()
        {
            List<FileItem> files = new();

            string[] filePaths = Directory.GetFiles(Path.Combine(MainModel.BaseFolder, "Dateien")); // Liest alle Dateipfade im Verzeichnis
            foreach (string filePath in filePaths)
            {
                FileInfo fi = new(filePath);
                if (fi.Extension == ".db-shm" || fi.Extension == ".db-wal") continue; // Überspringt bestimmte Dateitypen
                files.Add(new FileItem(fi.Name, fi.CreationTime)); // Fügt die Datei zur Liste hinzu
            }
            return files; // Gibt die Liste der Dateien zurück
        }
    }
}
