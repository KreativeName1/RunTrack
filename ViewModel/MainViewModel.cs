using System.Collections.ObjectModel;
using System.Windows;

namespace Klimalauf
{
    public class MainViewModel : BaseModel
    {
        private ObservableCollection<FileItem> _lstFiles = new();
        private ObservableCollection<Runde> _lstRunden = new();
        private ObservableCollection<Runde> _lstLetzteRunde = new();
        private Benutzer? _benutzer;
        private Window? _lastWindow;

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

        public ObservableCollection<FileItem> LstFiles
        {
            get
            {
                return this._lstFiles;
            }
            set
            {

                this._lstFiles = value;
                OnPropertyChanged("LstFiles");
            }
        }

        private ScanItem? _selSC;
        public ScanItem? selSC
        {
            get
            {
                return this._selSC;
            }

            set
            {
                this._selSC = value;

                this.isSCSelected = this._selSC != null;


                OnPropertyChanged("SelSC");
            }
        }

        private FileItem? _selFI;
        public FileItem? selFI
        {
            get
            {
                return this._selFI;
            }

            set
            {
                this._selFI = value;

                this.isFISelected = this._selFI != null;


                OnPropertyChanged("SelFI");
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

        private bool _isFISelected;
        public bool isFISelected
        {
            get
            {
                return _isFISelected;
            }

            set
            {
                this._isFISelected = value;
                OnPropertyChanged("IsFISelected");
            }

        }

        public Window? LastWindow
        {
            get
            {
                return this._lastWindow;
            }
            set
            {
                this._lastWindow = value;
                OnPropertyChanged("LastWindow");
            }
        }
    }
}
