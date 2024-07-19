using System.Collections.ObjectModel;

namespace Klimalauf
{
    public class ImportModel : BaseModel
    {
        private ObservableCollection<KlasseItem> _klasseItems = new();
        private ObservableCollection<RundenArt> _rundenArten = new();
        private RundenArt _selectedRundenArt = null;
        private KlasseItem _klasse = null;
        private Schule _schule = null;
        private string _neuSchuleName = "";
        private ObservableCollection<object> _CSVListe = new();

        private ObservableCollection<Schule> _schuleListe = new();

        private ObservableCollection<string> _reihenfolge = new();

        private string _pfad;
        public string Pfad
        {
            get { return _pfad; }
            set
            {
                _pfad = value;
                OnPropertyChanged("Pfad");
            }
        }


        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                SetProperty(ref _currentView, value);
            }
        }
        private Import1 _import1;
        private Import2 _import2;
        private Import3 _import3;
        public void ShowImport1()
        {
            CurrentView = _import1 ??= new Import1();
        }
        public void ShowImport2()
        {
            CurrentView = _import2 ??= new Import2();
        }
        public void ShowImport3()
        {
            CurrentView = _import3 ??= new Import3();
        }
        public void CloseWindow()
        {
            CurrentView = null;
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

        public string NeuSchuleName
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


        public RundenArt SelectedRundenArt
        {
            get { return _selectedRundenArt; }
            set
            {
                _selectedRundenArt = value;
                OnPropertyChanged("SelectedRundenArt");
            }
        }

        public KlasseItem Klasse
        {
            get { return _klasse; }
            set
            {
                _klasse = value;
                OnPropertyChanged("Klasse");
            }
        }

        public Schule Schule
        {
            get { return _schule; }
            set
            {
                _schule = value;
                OnPropertyChanged("Schule");
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
    public class KlasseItem
    {
        public string Bezeichnung { get; set; }
        public RundenArt RundenArt { get; set; }
    }
}
