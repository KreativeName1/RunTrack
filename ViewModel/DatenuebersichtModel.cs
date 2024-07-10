using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
    internal class DatenuebersichtModel : BaseModel
    {
        private ObservableCollection<Schule> _lstSchule = new ObservableCollection<Schule>();
        private ObservableCollection<Schueler> _lstSchueler = new ObservableCollection<Schueler>();
        private ObservableCollection<Klasse> _lstKlasse = new ObservableCollection<Klasse>();
        private ObservableCollection<Runde> _lstRunde = new ObservableCollection<Runde>();

        public ObservableCollection<Schule> LstSchule
        {
            get
            {
                return _lstSchule;
            }
            set
            {
                this._lstSchule = value;
                OnPropertyChanged("LstSchule");
            }
        }

        public ObservableCollection<Schueler> LstSchueler
        {
            get
            {
                return _lstSchueler;
            }
            set
            {
                this._lstSchueler = value;
                OnPropertyChanged("LstSchueler");
            }
        }

        public ObservableCollection<Klasse> LstKlasse
        {
            get
            {
                return this._lstKlasse;
            }
            set
            {
                this._lstKlasse = value;
                OnPropertyChanged("LstKlasse");
            }
        }

        public ObservableCollection<Runde> LstRunde
        {
            get
            {
                return _lstRunde;
            }
            set
            {
                this._lstRunde = value;
                OnPropertyChanged("LstRunde");
            }
        }

        public DatenuebersichtModel()
        {

        }
    }
}
