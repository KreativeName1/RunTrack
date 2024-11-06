using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace RunTrack
{
    public class StartseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ObservableCollection<Schueler> _lstSchueler { get; set; }
        private Schueler? _selSchueler;
        private bool _isLoading { get; set; }


        public LaufDBContext Db
        {
            get { return _db; }
            set
            {
                _db = value;
                OnPropertyChanged("Db");
            }
        }

        public ObservableCollection<Schueler> LstSchueler
        {
            get { return _lstSchueler; }
            set
            {
                _lstSchueler = value;
                OnPropertyChanged("LstSchueler");
            }
        }

        public Schueler? SelSchueler
        {
            get { return _selSchueler; }
            set
            {
                _selSchueler = value;
                OnPropertyChanged("SelSchueler");
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

        public StartseiteModel()
        {
            LoadData();
        }

        public void LoadData()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                _db = new();
                LstSchueler = new(_db.Schueler
                    .Include(s => s.Klasse)
                    .ThenInclude(k => k.Schule)
                    .Include(s => s.Klasse)
                    .ThenInclude(k => k.RundenArt)
                    .ToList());
                IsLoading = false;
            });
        }
    }
}
