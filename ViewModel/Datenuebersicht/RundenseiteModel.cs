using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace RunTrack
{
    public class RundenseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ObservableCollection<Runde> _lstRunde { get; set; }
        private Runde _selRunde { get; set; }
        private bool _hasChanges { get; set; }

        public Runde SelRunde
        {
            get { return _selRunde; }
            set
            {
                _selRunde = value;
                OnPropertyChanged("SelRunde");
            }
        }

        public ObservableCollection<Runde> LstRunde
        {
            get { return _lstRunde; }
            set
            {
                _lstRunde = value;
                OnPropertyChanged("LstRunde");
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



        public LaufDBContext Db
        {
            get { return _db; }
            set
            {
                _db = value;
                OnPropertyChanged("Db");
            }
        }


        public RundenseiteModel()
        {
            LoadData();
        }

        public void SaveChanges()
        {
            var added = LstRunde.ToList().Except(Db.Runden.AsEnumerable()).ToList();
            var removed = Db.Runden.AsEnumerable().Except(LstRunde.ToList()).ToList();
            var modified = LstRunde.ToList().Intersect(Db.Runden.AsEnumerable()).ToList();

            foreach (var item in added)
            {
                Db.Runden.Add(item);
            }

            foreach (var item in removed)
            {
                Db.Runden.Remove(item);
            }

            foreach (var item in modified)
            {
                Db.Entry(item).State = EntityState.Modified;
            }

            Db.SaveChanges();

            HasChanges = false;
        }

        public void Validate(Runde item)
        {
            if (item.Laeufer == null || item.LaeuferId == 0) throw new ValidationException("Laeufer darf nicht leer sein");
            if (item.Zeitstempel == null) throw new ValidationException("Zeitstempel darf nicht leer sein");
            if (item.Zeitstempel > DateTime.Now) throw new ValidationException("Zeitstempel darf nicht in der Zukunft liegen");
            if (string.IsNullOrWhiteSpace(item.BenutzerName)) throw new ValidationException("BenutzerName darf nicht leer sein");
        }

        public void LoadData()
        {
            Task.Run(() =>
            {
                _db = new();
                LstRunde = new(_db.Runden.Include(x => x.Laeufer).ToList());
            });
        }
    }
}
