using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    public class SchulenseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ICollectionView? _collectionView { get; set; }
        private ObservableCollection<Schule> _lstSchule { get; set; }
        private Schule _selSchule { get; set; }
        private bool _hasChanges { get; set; }
        private bool _isLoading { get; set; }
        private string _searchTerm { get; set; }

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
            if (item is Schule schule)
            {
                if (string.IsNullOrEmpty(SearchTerm)) return true;
                if (schule.Id.ToString().Contains(SearchTerm)) return true;
                if (schule.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
            }
            return false;
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
            IsLoading = true;
            Task.Run(() =>
            {
                _db = new();
                LstSchule = new(_db.Schulen);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsLoading = false;
                    CollectionView = CollectionViewSource.GetDefaultView(LstSchule);
                    CollectionView.Filter = FilterItems;
                });
            });
        }
    }
}
