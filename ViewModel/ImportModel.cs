using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RunTrack
{
    public class ImportModel : BaseModel
    {
        private RundenArt? _selectedRundenArt;
        private KlasseItem? _klasse;
        private Schule? _schule;
        private string? _neuSchuleName;

        private ObservableCollection<object> _CSVListe = new();
        private ObservableCollection<RundenArt> _rundenArten = new();
        private ObservableCollection<KlasseItem> _klasseItems = new();
        private ObservableCollection<Schule> _schuleListe = new();
        private ObservableCollection<string> _reihenfolge = new();

        public bool IsNeueSchule
        {
            get
            {
                return Schule != null && Schule.Id == 0;
            }
        }

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
        public void CloseWindow()
        {
        }


        public ObservableCollection<KlasseItem> KlasseItems
        {
            get { return _klasseItems; }
            set
            {
                _klasseItems = value;
                OnPropertyChanged("KlasseItems");
            }
        }

        public string? NeuSchuleName
        {
            get { return _neuSchuleName; }
            set
            {
                _neuSchuleName = value;
                OnPropertyChanged("NewSchoolName");
            }
        }

        public ObservableCollection<RundenArt> RundenArten
        {
            get { return _rundenArten; }
            set
            {
                _rundenArten = value;
                OnPropertyChanged("RundenArten");
            }
        }


        public RundenArt? SelectedRundenArt
        {
            get { return _selectedRundenArt; }
            set
            {
                _selectedRundenArt = value;
                OnPropertyChanged("SelectedRundenArt");
            }
        }

        public KlasseItem? Klasse
        {
            get { return _klasse; }
            set
            {
                _klasse = value;
                OnPropertyChanged("Klasse");
            }
        }

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

        public ObservableCollection<object> CSVListe
        {
            get { return _CSVListe; }
            set
            {
                _CSVListe = value;
                OnPropertyChanged("CSVListe");
            }
        }

        public ObservableCollection<Schule> SchuleListe
        {
            get { return _schuleListe; }
            set
            {
                _schuleListe = value;
                OnPropertyChanged("SchuleListe");
            }
        }

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
    public class KlasseItem : INotifyPropertyChanged
    {
        public string? Bezeichnung { get; set; }
        private RundenArt? _rundenArt { get; set; }
        public RundenArt? RundenArt
        {
            get { return _rundenArt; }
            set
            {
                _rundenArt = value;
                OnPropertyChanged("RundenArt");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
