using System.Collections.ObjectModel;

namespace RunTrack
{
    // Modellklasse zur Verwaltung von Dateien, erbt von BaseModel
    internal class DateiVerwaltungModel : BaseModel
    {
        // Liste der Dateien als ObservableCollection
        private ObservableCollection<FileItem> _lstFiles = new();
        // Status, ob gerade eine Datei gezogen wird
        private bool _isDragging = false;

        // Öffentliche Eigenschaft für den Dragging-Status mit Benachrichtigung bei Änderung
        public bool IsDragging
        {
            get
            {
                return _isDragging;
            }
            set
            {
                _isDragging = value;
                // Benachrichtigung, dass sich die Eigenschaft geändert hat
                OnPropertyChanged("IsDragging");
            }
        }

        // Öffentliche Eigenschaft für die Liste der Dateien mit Benachrichtigung bei Änderung
        public ObservableCollection<FileItem> LstFiles
        {
            get
            {
                return this._lstFiles;
            }
            set
            {
                this._lstFiles = value;
                // Benachrichtigung, dass sich die Eigenschaft geändert hat
                OnPropertyChanged("LstFiles");
            }
        }
    }
}
