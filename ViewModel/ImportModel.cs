using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RunTrack
{
    // ImportModel-Klasse, die von BaseModel erbt und verschiedene Eigenschaften und Sammlungen für die Importfunktionalität bereitstellt
    public class ImportModel : BaseModel
    {
        // Private Felder für die ausgewählte RundenArt, Klasse, Schule und den neuen Schulnamen
        private RundenArt? _selectedRundenArt;
        private KlasseItem? _klasse;
        private Schule? _schule;
        private string? _neuSchuleName;

        // Private ObservableCollection-Felder für CSV-Liste, RundenArten, KlasseItems, SchuleListe und Reihenfolge
        private ObservableCollection<object> _CSVListe = new();
        private ObservableCollection<RundenArt> _rundenArten = new();
        private ObservableCollection<KlasseItem> _klasseItems = new();
        private ObservableCollection<Schule> _schuleListe = new();
        private ObservableCollection<string> _reihenfolge = new();

        // Eigenschaft, die überprüft, ob eine neue Schule erstellt wird
        public bool IsNeueSchule
        {
            get
            {
                return Schule != null && Schule.Id == 0;
            }
        }

        // Private Eigenschaft für den Pfad
        private string? _pfad;
        public string? Pfad
        {
            get { return _pfad; }
            set
            {
                _pfad = value;
                OnPropertyChanged("Pfad");
            }
        }

        // Methode zum Schließen des Fensters (derzeit leer)
        public void CloseWindow()
        {
        }

        // Eigenschaft für die KlasseItems-Sammlung
        public ObservableCollection<KlasseItem> KlasseItems
        {
            get { return _klasseItems; }
            set
            {
                _klasseItems = value;
                OnPropertyChanged("KlasseItems");
            }
        }

        // Eigenschaft für den neuen Schulnamen
        public string? NeuSchuleName
        {
            get { return _neuSchuleName; }
            set
            {
                _neuSchuleName = value;
                OnPropertyChanged("NewSchoolName");
            }
        }

        // Eigenschaft für die RundenArten-Sammlung
        public ObservableCollection<RundenArt> RundenArten
        {
            get { return _rundenArten; }
            set
            {
                _rundenArten = value;
                OnPropertyChanged("RundenArten");
            }
        }

        // Eigenschaft für die ausgewählte RundenArt
        public RundenArt? SelectedRundenArt
        {
            get { return _selectedRundenArt; }
            set
            {
                _selectedRundenArt = value;
                OnPropertyChanged("SelectedRundenArt");
            }
        }

        // Eigenschaft für die Klasse
        public KlasseItem? Klasse
        {
            get { return _klasse; }
            set
            {
                _klasse = value;
                OnPropertyChanged("Klasse");
            }
        }

        // Eigenschaft für die Schule
        public Schule? Schule
        {
            get { return _schule; }
            set
            {
                _schule = value;
                OnPropertyChanged("Schule");
                OnPropertyChanged("isNeueSchule");
            }
        }

        // Eigenschaft für die CSV-Liste
        public ObservableCollection<object> CSVListe
        {
            get { return _CSVListe; }
            set
            {
                _CSVListe = value;
                OnPropertyChanged("CSVListe");
            }
        }

        // Eigenschaft für die SchuleListe-Sammlung
        public ObservableCollection<Schule> SchuleListe
        {
            get { return _schuleListe; }
            set
            {
                _schuleListe = value;
                OnPropertyChanged("SchuleListe");
            }
        }

        // Eigenschaft für die Reihenfolge-Sammlung
        public ObservableCollection<string> Reihenfolge
        {
            get { return _reihenfolge; }
            set
            {
                _reihenfolge = value;
                OnPropertyChanged("Reihenfolge");
            }
        }
    }
    // Klasse für KlasseItem, die INotifyPropertyChanged implementiert
    public class KlasseItem : INotifyPropertyChanged
    {
        // Eigenschaft für die Bezeichnung der Klasse
        public string? Bezeichnung { get; set; }
        // Private Eigenschaft für die RundenArt
        private RundenArt? _rundenArt { get; set; }
        // Eigenschaft für die RundenArt
        public RundenArt? RundenArt
        {
            get { return _rundenArt; }
            set
            {
                _rundenArt = value;
                OnPropertyChanged("RundenArt");
            }
        }

        // Event für PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        // Methode zum Auslösen des PropertyChanged-Ereignisses
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
