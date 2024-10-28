using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace RunTrack
{
    public class StartseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ObservableCollection<Schueler> _lstSchueler { get; set; }
        private Schueler? _selSchueler;

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

        public StartseiteModel()
        {
            LoadData();
        }

        public void LoadData()
        {
            _db = new();
            _lstSchueler = new(_db.Schueler
                .Include(s => s.Klasse)
                .ThenInclude(k => k.Schule)
                .Include(s => s.Klasse)
                .ThenInclude(k => k.RundenArt)
                .ToList());
        }
    }
}
