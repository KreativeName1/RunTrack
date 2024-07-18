using System.Collections.ObjectModel;

namespace Klimalauf
{
    public class ImportModel : BaseModel
    {
        private ObservableCollection<KlasseItem> _klasseItems;
        private ObservableCollection<RundenArt> _rundenArten;
        private KlasseItem _klasse;
        private Schule _schule;
        private ObservableCollection<object> _CSVListe;

        private ObservableCollection<Schule> _schuleListe;

        private ObservableCollection<string> _reihenfolge;

        public ObservableCollection<KlasseItem> KlasseItems
        {
            get { return _klasseItems; }
            set
            {
                _klasseItems = value;
                OnPropertyChanged("KlasseItems");
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
        public string Name { get; set; }
        public string RundenArt { get; set; }
    }
}
