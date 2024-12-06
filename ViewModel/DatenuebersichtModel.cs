using System.Windows.Controls;

namespace RunTrack
{
    // Datenmodell für die Datenübersicht
    public class DatenuebersichtModel : BaseModel
    {
        // Aktuelle Seite
        private Page? _currentPage;

        // Gibt an, ob Änderungen vorhanden sind
        private bool _hasChanges;

        // Gibt an, ob das Modell schreibgeschützt ist
        private bool _readOnly = false;

        // Verbindungszeichenfolge zur Datenbank
        private string? connectionString;

        // Eigenschaft für die Verbindungszeichenfolge
        public string? ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                this.connectionString = value;
                // Benachrichtigung über Eigenschaftsänderung
                OnPropertyChanged("ConnectionString");
            }
        }

        // Eigenschaft für den Schreibschutz
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                this._readOnly = value;
                // Benachrichtigung über Eigenschaftsänderung
                OnPropertyChanged("ReadOnly");
            }
        }

        // Eigenschaft für Änderungen
        public bool HasChanges
        {
            get
            {
                return _hasChanges;
            }
            set
            {
                this._hasChanges = value;
                // Benachrichtigung über Eigenschaftsänderung
                OnPropertyChanged("HasChanges");
            }
        }

        // Eigenschaft für die aktuelle Seite
        public Page? CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                this._currentPage = value;
                // Benachrichtigung über Eigenschaftsänderung
                OnPropertyChanged("CurrentPage");
            }
        }
    }
}
