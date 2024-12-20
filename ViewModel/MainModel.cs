using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    // Die MainModel-Klasse erbt von BaseModel und stellt die Hauptlogik für die Anwendung bereit
    public class MainModel : BaseModel
    {
        // Private Felder für die aktuelle Seite, den Seitentitel und die Verlaufsliste
        private object? _currentPage;
        private string? _pageTitle = "RunTrack";
        private List<object> _history = new List<object>();
        private bool _showTimeWarningPopup = false;

        // Öffentliche Eigenschaft, um zu bestimmen, ob das TimeWarningPopup angezeigt werden soll
        public bool ShowTimeWarningPopup
        {
            get => _showTimeWarningPopup;
            set
            {
                _showTimeWarningPopup = value;
                OnPropertyChanged(nameof(ShowTimeWarningPopup));
            }
        }

        // Öffentliche Eigenschaft für die aktuelle Seite mit Benachrichtigung bei Änderung
        public object? CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        // Private Variable für den Benutzer
        private Benutzer? _benutzer;

        // Öffentliche Eigenschaft für den Benutzer mit Standardwert und Benachrichtigung bei Änderung
        public Benutzer? Benutzer
        {
            get
            {
                return this._benutzer ?? new Benutzer();
            }
            set
            {
                this._benutzer = value;
                OnPropertyChanged("Benutzer");
            }
        }

        // Öffentliche Eigenschaft für den Verlauf mit Benachrichtigung bei Änderung
        public List<object> History
        {
            get { return _history; }
            set
            {
                if (!_history.Contains(value))
                    _history = value;
                OnPropertyChanged("History");
            }
        }

        // Öffentliche Eigenschaft für den Seitentitel mit Benachrichtigung bei Änderung
        public string? PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                OnPropertyChanged("PageTitle");
            }
        }

        public static string BaseFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RunTrack");


        // Methode zum Navigieren zu einer neuen Seite mit optionalem Hinzufügen zur Verlaufsliste und Bedingung
        public void Navigate(object page, bool? addToHistory = true, bool? condition = true)
        {
            if (condition == false) return; // Abbrechen, wenn die Bedingung nicht erfüllt ist
            if (page == null) return; // Abbrechen, wenn die Seite null ist
            if (CurrentPage != null && addToHistory == true)
            {
                History.Add(CurrentPage); // Aktuelle Seite zur Verlaufsliste hinzufügen
            }
            CurrentPage = page; // Neue Seite setzen
            PageTitle = (CurrentPage as Page)?.Title; // Seitentitel aktualisieren

            Window window = Application.Current.MainWindow; // Hauptfenster der Anwendung
        }
    }
}
