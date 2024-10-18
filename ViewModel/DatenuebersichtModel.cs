using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace RunTrack
{
    public class DatenuebersichtModel : BaseModel
    {
        private ObservableCollection<Schule> _lstSchule = new();
        private ObservableCollection<Schueler> _lstSchueler = new();
        private ObservableCollection<Klasse> _lstKlasse = new();
        private ObservableCollection<Runde> _lstRunde = new();
        private ObservableCollection<RundenArt> _lstRundenArt = new();
        private Page? _currentPage;

        // selected items
        private Schule? _selSchule;
        private Klasse? _selKlasse;
        private Runde? _selRunde;
        private Schueler? _selSchueler;
        private RundenArt? _selRundenArt;


        private LaufDBContext context = new();

        public Page? CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                this._currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

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

        public ObservableCollection<RundenArt> LstRundenArt
        {
            get
            {
                return _lstRundenArt;
            }
            set
            {
                this._lstRundenArt = value;
                OnPropertyChanged("LstRundenArt");
            }
        }

        public RundenArt? SelRundenArt
        {
            get
            {
                return _selRundenArt;
            }
            set
            {
                this._selRundenArt = value;
                OnPropertyChanged("SelRundenArt");
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
        public void LoadData()
        {
            LstSchule = new ObservableCollection<Schule>(context.Schulen.ToList());
            LstKlasse = new ObservableCollection<Klasse>(context.Klassen.Include(k => k.Schule).Include(r => r.RundenArt).Include(k => k.Schueler).ThenInclude(s => s.Runden).ToList());
            LstSchueler = new ObservableCollection<Schueler>(context.Schueler.Include(s => s.Klasse).ThenInclude(k => k.Schule).Include(s => s.Runden).ToList());
            LstRunde = new ObservableCollection<Runde>(context.Runden.Include(r => r.Schueler).ThenInclude(s => s.Klasse).ThenInclude(k => k.Schule).ToList());
            LstRundenArt = new ObservableCollection<RundenArt>(context.RundenArten.ToList());
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

        public Schule? SelSchule
        {
            get
            {
                return _selSchule;
            }
            set
            {
                this._selSchule = value;
                OnPropertyChanged("SelSchule");
            }
        }
        public Klasse? SelKlasse
        {
            get
            {
                return _selKlasse;
            }
            set
            {
                this._selKlasse = value;
                OnPropertyChanged("SelKlasse");
            }
        }
        public Runde? SelRunde
        {
            get
            {
                return _selRunde;
            }
            set
            {
                this._selRunde = value;
                OnPropertyChanged("SelRunde");
            }
        }
        public Schueler? SelSchueler
        {
            get
            {
                return _selSchueler;
            }
            set
            {
                this._selSchueler = value;
                OnPropertyChanged("SelSchueler");
            }
        }
    }
}
