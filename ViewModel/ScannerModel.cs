using System.Collections.ObjectModel;

namespace RunTrack
{
    // ScannerModel erbt von BaseModel und implementiert INotifyPropertyChanged
    public class ScannerModel : BaseModel
    {
        // Liste der Runden
        private ObservableCollection<Runde> _lstRunden = new();
        // Liste der letzten Runde
        private ObservableCollection<Runde> _lstLetzteRunde = new();
        // Manuelle Eingabe
        private int _manuelleEingabe = 0;

        // Öffentliche Eigenschaft für die Liste der Runden
        public ObservableCollection<Runde> LstRunden
        {
            get
            {
                return this._lstRunden;
            }
            set
            {
                this._lstRunden = value;
                // Benachrichtigung, dass sich die Eigenschaft geändert hat
                OnPropertyChanged("LstRunden");
            }
        }

        // Öffentliche Eigenschaft für die Liste der letzten Runde (nur lesbar)
        public ObservableCollection<Runde> LstLetzteRunde
        {
            get
            {
                return this._lstLetzteRunde;
            }
        }

        // Öffentliche Eigenschaft für die manuelle Eingabe
        public int ManuelleEingabe
        {
            get
            {
                return this._manuelleEingabe;
            }
            set
            {
                this._manuelleEingabe = value;
                // Benachrichtigung, dass sich die Eigenschaft geändert hat
                OnPropertyChanged("ManuelleEingabe");
            }
        }

        // Methode zum Hinzufügen der letzten Runde
        public void hinzufügeLetzteRunde(Runde runde)
        {
            // Liste der letzten Runde leeren
            _lstLetzteRunde.Clear();
            // Neue Runde hinzufügen
            _lstLetzteRunde.Add(runde);
            // Benachrichtigung, dass sich die Eigenschaft geändert hat
            OnPropertyChanged("LstLetzteRunden");
        }
    }
}
