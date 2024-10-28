using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace RunTrack
{
    public class SchuelerseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ObservableCollection<Schueler> _lstSchueler { get; set; }
        private ObservableCollection<Schule> _lstSchule { get; set; }
        private ObservableCollection<Klasse> _lstKlasse { get; set; }
        private Schueler _selSchueler { get; set; }
        private bool _hasChanges { get; set; }

        public ObservableCollection<Schueler> LstSchueler
        {
            get { return _lstSchueler; }
            set
            {
                _lstSchueler = value;
                OnPropertyChanged("LstSchueler");
            }
        }

        public Schueler SelSchueler
        {
            get { return _selSchueler; }
            set
            {
                _selSchueler = value;
                OnPropertyChanged("SelSchueler");
            }
        }

        public ObservableCollection<Klasse> LstKlasse
        {
            get { return _lstKlasse; }
            set
            {
                _lstKlasse = value;
                OnPropertyChanged("LstKlasse");
            }
        }

        public ObservableCollection<Schule> LstSchule
        {
            get { return _lstSchule; }
            set
            {
                _lstSchule = value;
                OnPropertyChanged("LstSchule");
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


        public SchuelerseiteModel()
        {
            LoadData();
        }

        public void SaveChanges()
        {
            var added = LstSchueler.ToList().Except(Db.Schueler.AsEnumerable()).ToList();
            var removed = Db.Schueler.AsEnumerable().Except(LstSchueler.ToList()).ToList();
            var modified = LstSchueler.ToList().Intersect(Db.Schueler.AsEnumerable()).ToList();

            // Perform the necessary actions
            foreach (var item in added)
            {
                item.Klasse = Db.Klassen.Find(1);

                Db.Schueler.Add(item);
            }

            foreach (var item in removed)
            {
                Db.Schueler.Remove(item);
            }

            foreach (var item in modified)
            {
                Db.Entry(item).State = EntityState.Modified;
            }

            Db.SaveChanges();

            HasChanges = false;
        }

        public void LoadData()
        {
            _db = new();
            _db.ChangeTracker.DetectChanges();
            LstSchueler = new();
            LstSchueler = new(_db.Schueler.Include(s => s.Klasse).ThenInclude(k => k.Schule).Include(s => s.Runden).ToList());
            LstSchule = new(_db.Schulen.ToList());
            LstKlasse = new(_db.Klassen.ToList());
        }
    }
}
