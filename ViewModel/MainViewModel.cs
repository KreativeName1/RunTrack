using System.Collections.ObjectModel;

namespace RunTrack
{
    public class MainViewModel : BaseModel
    {
        private ObservableCollection<Runde> _lstRunden = new();
        private ObservableCollection<Runde> _lstLetzteRunde = new();
        private Benutzer? _benutzer;

        public Benutzer Benutzer
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

        public ObservableCollection<Runde> LstRunden
        {
            get
            {
                return this._lstRunden;
            }
            set
            {

                this._lstRunden = value;
                OnPropertyChanged("LstRunden");
            }
        }

        public ObservableCollection<Runde> LstLetzteRunde
        {
            get
            {
                return this._lstLetzteRunde;
            }
        }

        public void hinzufügeLetzteRunde(Runde runde)
        {
            _lstLetzteRunde.Clear();
            _lstLetzteRunde.Add(runde);
            OnPropertyChanged("LstLetzteRunden");


        }

        private ScanItem? _selScanItem;
        public ScanItem? SelScanItem
        {
            get
            {
                return this._selScanItem;
            }

            set
            {
                this._selScanItem = value;

                this.isSCSelected = this._selScanItem != null;


                OnPropertyChanged("SelScanItem");
            }
        }

        private bool _isSCSelected;
        public bool isSCSelected
        {
            get
            {
                return _isSCSelected;
            }

            set
            {
                this._isSCSelected = value;
                OnPropertyChanged("IsSCSelected");
            }

        }
    }
}
