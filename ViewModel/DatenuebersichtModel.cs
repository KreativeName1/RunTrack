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
        private string[] pfade = System.IO.Directory.GetFiles("Dateien", "*.db");

        private ObservableCollection<Schule> _lstSchule = new ObservableCollection<Schule>();
        private ObservableCollection<Schueler> _lstSchueler = new ObservableCollection<Schueler>();
        private ObservableCollection<Klasse> _lstKlasse = new ObservableCollection<Klasse>();
        private ObservableCollection<Runde> _lstRunde = new ObservableCollection<Runde>();

        public ObservableCollection<Schule> LstSchule
        {
            get
            {
                using(var db = new MergedDBContext(pfade)) 
                {
                    return new ObservableCollection<Schule>(db.Schulen.ToList());
                }
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
                using (var db = new LaufDBContext())
                {
                    return new ObservableCollection<Schueler>(db.Schueler.ToList());
                }
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
                using (var db = new LaufDBContext())
                {
                    return new ObservableCollection<Klasse>(db.Klassen.ToList());
                }
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
                using (var db = new LaufDBContext())
                {
                    return new ObservableCollection<Runde>(db.Runden.ToList());
                }
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
