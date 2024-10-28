using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace RunTrack
{
    public class KlassenseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ObservableCollection<Klasse> _lstKlasse { get; set; }
        private ObservableCollection<Schule> _lstSchule { get; set; }
        private ObservableCollection<RundenArt> _lstRundenart { get; set; }
        private Klasse _selKlasse { get; set; }
        private bool _hasChanges { get; set; }

        public ObservableCollection<Klasse> LstKlasse
        {
            get { return _lstKlasse; }
            set
            {
                _lstKlasse = value;
                OnPropertyChanged("LstKlasse");
            }
        }

        public Klasse SelKlasse
        {
            get { return _selKlasse; }
            set
            {
                _selKlasse = value;
                OnPropertyChanged("SelKlasse");
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

        public ObservableCollection<RundenArt> LstRundenart
        {
            get { return _lstRundenart; }
            set
            {
                _lstRundenart = value;
                OnPropertyChanged("LstRundenart");
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


        public KlassenseiteModel()
        {
            LoadData();
        }

        public void SaveChanges()
        {
            var added = LstKlasse.ToList().Except(Db.Klassen.AsEnumerable()).ToList();
            var removed = Db.Klassen.AsEnumerable().Except(LstKlasse.ToList()).ToList();
            var modified = LstKlasse.ToList().Intersect(Db.Klassen.AsEnumerable()).ToList();

            foreach (var item in added)
            {
                Validate(item);
                Db.Klassen.Add(item);
            }

            foreach (var item in removed)
            {
                Validate(item);
                Db.Klassen.Remove(item);
            }

            foreach (var item in modified)
            {
                Db.Entry(item).State = EntityState.Modified;
            }

            Db.SaveChanges();

            HasChanges = false;
        }

        public void Validate(Klasse klasse)
        {
            if (klasse.Schule == null || klasse.Schule.Id == 0) throw new ValidationException("Schule darf nicht leer sein");

            if (klasse.RundenArt == null || klasse.RundenArt.Id == 0) throw new ValidationException("Rundenart darf nicht leer sein");

            if (string.IsNullOrWhiteSpace(klasse.Name)) throw new ValidationException("Name darf nicht leer sein");
        }

        public void LoadData()
        {

            Task.Run(() =>
            {
                _db = new();
                LstSchule = new(_db.Schulen.ToList());
                LstKlasse = new(_db.Klassen.Include(k => k.Schueler).ToList());
                LstRundenart = new(_db.RundenArten.ToList());
            });
        }
    }
}
