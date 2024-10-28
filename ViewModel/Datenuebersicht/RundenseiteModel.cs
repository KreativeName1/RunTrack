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

        public void LoadData()
        {
            _db = new();
            _db.ChangeTracker.DetectChanges();
            LstRunde = new(_db.Runden.Include(x => x.Laeufer).ToList());
        }
    }
}
