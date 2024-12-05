using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    public class StartseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ICollectionView? _collectionView { get; set; }
        private ObservableCollection<Schueler> _lstSchueler { get; set; }
        private Schueler? _selSchueler;
        private bool _isLoading { get; set; }
        private string _searchTerm { get; set; }
        private bool _readOnly { get; set; }

        public bool ReadOnly
        {
            get { return ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ReadOnly ? true : false; }
            set
            {
                ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ReadOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }

        public string? ConnectionString => ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ConnectionString;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    OnPropertyChanged("SearchTerm");
                    CollectionView?.Refresh();
                }
            }
        }

        public ICollectionView CollectionView
        {
            get { return _collectionView; }
            set
            {
                _collectionView = value;
                OnPropertyChanged("CollectionView");
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
        private bool FilterItems(object item)
        {
            if (item is Schueler schueler)
            {
                if (string.IsNullOrEmpty(SearchTerm)) return true;
                if (schueler.Id.ToString().Contains(SearchTerm)) return true;
                if (schueler.Vorname.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Nachname.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Klasse.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Klasse.Schule.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Klasse.RundenArt.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Geschlecht.ToString().ToLower().Contains(SearchTerm.ToLower())) return true;
            }
            return false;
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
                if (ReadOnly) _db = new(ConnectionString);
                else _db = new();
                LstSchueler = new(_db.Schueler
                    .Include(s => s.Klasse)
                    .ThenInclude(k => k.Schule)
                    .Include(s => s.Klasse)
                    .ThenInclude(k => k.RundenArt)
                    .ToList());

                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsLoading = false;
                    CollectionView = CollectionViewSource.GetDefaultView(LstSchueler);
                    CollectionView.Filter = FilterItems;
                });

            });
        }
    }
}
