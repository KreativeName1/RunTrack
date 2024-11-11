using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace RunTrack
{
    public class RundenseiteModel : BaseModel
    {
        private LaufDBContext? _db;
        private ICollectionView? _collectionView { get; set; }
        private ObservableCollection<Runde> _lstRunde { get; set; }
        private Runde _selRunde { get; set; }
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

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
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

        private bool FilterItems(object item)
        {
            if (item is Runde runde)
            {
                if (string.IsNullOrEmpty(SearchTerm)) return true;
                if (runde.Id.ToString().Contains(SearchTerm)) return true;
                if (runde.BenutzerName.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (runde.Laeufer.Vorname.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (runde.Laeufer.Nachname.ToLower().Contains(SearchTerm.ToLower())) return true;
                //if (runde.Laeufer is Schueler schueler)
                //{
                //    if (schueler.Klasse != null && schueler.Klasse.RundenArt.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
                //}
                //else if (runde.Laeufer is Laeufer laeufer)
                //{
                //    if (laeufer.RundenArt.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
                //}
            }
            return false;
        }

        public RundenseiteModel()
        {
            LoadData();
        }

        public void LoadData()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                _db = new();
                LstRunde = new(_db.Runden.Include(x => x.Laeufer).ToList());
                CollectionView = CollectionViewSource.GetDefaultView(LstRunde);
                CollectionView.Filter = FilterItems;
                IsLoading = false;
            });
        }
    }
}
