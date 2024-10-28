using System.Collections.ObjectModel;

namespace RunTrack
{
    public class LaeuferseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ObservableCollection<Laeufer> _lstLaeufer { get; set; }
        private ObservableCollection<RundenArt> _lstRundenart { get; set; }
        private Laeufer _selLaeufer { get; set; }
        private RundenArt _selRundenArt { get; set; }
        private bool _hasChanges { get; set; }

        public ObservableCollection<Laeufer> LstLaeufer
        {
            get { return _lstLaeufer; }
            set
            {
                _lstLaeufer = value;
                OnPropertyChanged("LstLaeufer");
            }
        }

        public Laeufer SelLaeufer
        {
            get { return _selLaeufer; }
            set
            {
                _selLaeufer = value;
                OnPropertyChanged("SelLaeufer");
            }
        }

        public LaufDBContext Db
        {
            get { return _db; }
            set
            {
                _db = value;
                OnPropertyChanged("Db");
            }
        }

        public ObservableCollection<RundenArt> LstRundenart
        {
            get { return _lstRundenart; }
            set
            {
                _lstRundenart = value;
                OnPropertyChanged("LstRundenart");
            }
        }

        public RundenArt SelRundenArt
        {
            get { return _selRundenArt; }
            set
            {
                _selRundenArt = value;
                OnPropertyChanged("SelRundenArt");
            }
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                OnPropertyChanged("HasChanges");
                ((DatenuebersichtModel)App.Current.Resources["dumodel"]).HasChanges = value;
            }
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
            HasChanges = false;
        }

        public LaeuferseiteModel()
        {
            _db = new();
            LstLaeufer = new(_db.Laeufer.Where(x => x.RundenArt != null));
            LstRundenart = new(_db.RundenArten);
        }
    }
}
