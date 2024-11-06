using Microsoft.EntityFrameworkCore;
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
        private bool _isLoading { get; set; }


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
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }
        public void SaveChanges()
        {
            var added = LstLaeufer.ToList().Except(Db.Laeufer.AsEnumerable()).ToList();
            var removed = Db.Laeufer.AsEnumerable().Except(LstLaeufer.ToList()).ToList();
            var modified = LstLaeufer.ToList().Intersect(Db.Laeufer.AsEnumerable()).ToList();

            foreach (var item in added)
            {
                Validate(item);
                Db.Laeufer.Add(item);
            }

            foreach (var item in removed)
            {
                Db.Laeufer.Remove(item);
            }

            foreach (var item in modified)
            {
                Validate(item);
                Db.Entry(item).State = EntityState.Modified;
            }

            Db.SaveChanges();

            HasChanges = false;
        }

        public void Validate(Laeufer laeufer)
        {
            if (laeufer.RundenArt == null || laeufer.RundenArtId == 0) throw new ValidationException("Rundenart darf nicht leer sein");
            if (laeufer.Geschlecht == null) throw new ValidationException("Geschlecht darf nicht leer sein");
            if (string.IsNullOrWhiteSpace(laeufer.Vorname) || string.IsNullOrWhiteSpace(laeufer.Nachname) || laeufer.Geburtsjahrgang == 0) throw new ValidationException("Alle Felder müssen ausgefüllt werden");
        }

        public LaeuferseiteModel()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                _db = new();
                LstLaeufer = new(_db.Laeufer.Where(x => x.RundenArt != null));
                LstRundenart = new(_db.RundenArten);
                IsLoading = false;
            });
        }
    }
}
