using System.Collections.ObjectModel;

namespace RunTrack
{
    public class AuswertungModel : BaseModel
    {
        private bool _isMaennlich { get; set; } = false;
        private bool _isWeiblich { get; set; } = false;
        private bool _isDivers { get; set; } = false;
        private bool _isGesamt { get; set; } = true;

        private bool _isAnzahl { get; set; } = true;
        private bool _isZeit { get; set; } = false;
        private bool _isDistanz { get; set; } = false;

        private bool _isInsgesamt { get; set; } = true;
        private bool _isSchule { get; set; } = false;
        private bool _isKlasse { get; set; } = false;
        private bool _isJahrgang { get; set; } = false;
        private bool _isLaeufer { get; set; } = false;

        private ObservableCollection<object> _liste { get; set; } = new ObservableCollection<object>();
        private object? _selectedItem { get; set; } = null;

        private int _jahrgang { get; set; } = 2000;
        private ObservableCollection<Schule> _schulen { get; set; } = new ObservableCollection<Schule>();
        private Schule? _selectedSchule { get; set; } = null;
        private ObservableCollection<Klasse> _klassen { get; set; } = new ObservableCollection<Klasse>();
        private Klasse? _selectedKlasse { get; set; } = null;

        private bool _isLeerListe { get; set; } = false;

        public bool IsLeerListe
        {
            get { return _isLeerListe; }
            set { _isLeerListe = value; OnPropertyChanged("IsLeerListe"); }
        }



        public bool IsMaennlich
        {
            get { return _isMaennlich; }
            set { _isMaennlich = value; OnPropertyChanged("IsMaennlich"); }
        }
        public bool IsWeiblich
        {
            get { return _isWeiblich; }
            set { _isWeiblich = value; OnPropertyChanged("IsWeiblich"); }
        }
        public bool IsGesamt
        {
            get { return _isGesamt; }
            set { _isGesamt = value; OnPropertyChanged("IsGesamt"); }
        }
        public bool IsDivers
        {
            get { return _isDivers; }
            set { _isDivers = value; OnPropertyChanged("IsDivers"); }
        }


        public bool IsAnzahl
        {
            get { return _isAnzahl; }
            set { _isAnzahl = value; OnPropertyChanged("IsAnzahl"); }
        }
        public bool IsZeit
        {
            get { return _isZeit; }
            set { _isZeit = value; OnPropertyChanged("IsZeit"); }
        }
        public bool IsDistanz
        {
            get { return _isDistanz; }
            set { _isDistanz = value; OnPropertyChanged("IsDistanz"); }
        }


        public bool IsInsgesamt
        {
            get { return _isInsgesamt; }
            set { _isInsgesamt = value; OnPropertyChanged("IsInsgesamt"); }
        }
        public bool IsSchule
        {
            get { return _isSchule; }
            set { _isSchule = value; OnPropertyChanged("IsSchule"); }
        }
        public bool IsKlasse
        {
            get { return _isKlasse; }
            set { _isKlasse = value; OnPropertyChanged("IsKlasse"); }
        }
        public bool IsJahrgang
        {
            get { return _isJahrgang; }
            set { _isJahrgang = value; OnPropertyChanged("IsJahrgang"); }
        }
        public bool IsLaeufer
        {
            get { return _isLaeufer; }
            set { _isLaeufer = value; OnPropertyChanged("IsLaeufer"); }
        }


        public int Jahrgang
        {
            get { return _jahrgang; }
            set { _jahrgang = value; OnPropertyChanged("Jahrgang"); }
        }


        public ObservableCollection<Schule> Schulen
        {
            get { return _schulen; }
            set { _schulen = value; OnPropertyChanged("Schulen"); }
        }

        public Schule? SelectedSchule
        {
            get { return _selectedSchule; }
            set { _selectedSchule = value; OnPropertyChanged("SelectedSchule"); }
        }

        public ObservableCollection<Klasse> Klassen
        {
            get { return _klassen; }
            set { _klassen = value; OnPropertyChanged("Klassen"); }
        }

        public Klasse? SelectedKlasse
        {
            get { return _selectedKlasse; }
            set { _selectedKlasse = value; OnPropertyChanged("SelectedKlasse"); }
        }

        public ObservableCollection<object> Liste
        {
            get { return _liste; }
            set
            {
                _liste = value;
                OnPropertyChanged("Liste");
            }
        }

        public object? SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }
    }

}
