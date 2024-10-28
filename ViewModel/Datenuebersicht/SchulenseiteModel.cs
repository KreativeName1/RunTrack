using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace RunTrack
{
    public class SchulenseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ObservableCollection<Schule> _lstSchule { get; set; }
        private Schule _selSchule { get; set; }
        private bool _hasChanges { get; set; }

        public ObservableCollection<Schule> LstSchule
        {
            get { return _lstSchule; }
            set
            {
                _lstSchule = value;
                OnPropertyChanged("LstSchule");
            }
        }

        public Schule SelSchule
        {
            get { return _selSchule; }
            set
            {
                _selSchule = value;
                OnPropertyChanged("SelSchule");
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


        public SchulenseiteModel()
        {
            LoadData();
        }

        public void SaveChanges()
        {
            var added = LstSchule.ToList().Except(Db.Schulen.AsEnumerable()).ToList();
            var removed = Db.Schulen.AsEnumerable().Except(LstSchule.ToList()).ToList();
            var modified = LstSchule.ToList().Intersect(Db.Schulen.AsEnumerable()).ToList();

            // Perform the necessary actions
            foreach (var item in added)
            {
                Validate(item);
                Db.Schulen.Add(item);
            }

            foreach (var item in removed)
            {
                Db.Schulen.Remove(item);
            }

            foreach (var item in modified)
            {
                Validate(item);
                Db.Entry(item).State = EntityState.Modified;
            }

            Db.SaveChanges();

            HasChanges = false;
        }

        public void Validate(Schule schule)
        {
            if (string.IsNullOrEmpty(schule.Name)) throw new ValidationException("Name darf nicht leer sein");
        }

        public void LoadData()
        {
            Task.Run(() =>
            {
                _db = new();
                LstSchule = new(_db.Schulen);
            });
        }
    }
}
