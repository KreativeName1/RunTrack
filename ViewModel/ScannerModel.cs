using System.Collections.ObjectModel;

namespace RunTrack
{
    public class ScannerModel : BaseModel
    {
        private ObservableCollection<Runde> _lstRunden = new();
        private ObservableCollection<Runde> _lstLetzteRunde = new();
        private int _manuelleEingabe = 0;

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

        public int ManuelleEingabe
        {
            get
            {
                return this._manuelleEingabe;
            }
            set
            {
                this._manuelleEingabe = value;
                OnPropertyChanged("ManuelleEingabe");
            }
        }


        public void hinzufügeLetzteRunde(Runde runde)
        {
            _lstLetzteRunde.Clear();
            _lstLetzteRunde.Add(runde);
            OnPropertyChanged("LstLetzteRunden");
        }

    }
}
