using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    // Definiert das ViewModel für die Rundenseite
    public class RundenseiteModel : BaseModel
    {
        // Private Felder für die Datenbank, CollectionView, ObservableCollection und andere Eigenschaften
        private LaufDBContext? _db;
        private ICollectionView? _collectionView { get; set; }
        private ObservableCollection<Runde> _lstRunde { get; set; }
        private Runde _selRunde { get; set; }
        private bool _hasChanges { get; set; }
        private bool _isLoading { get; set; }
        private string _searchTerm { get; set; }
        private bool _readOnly { get; set; }

        // Öffentliche Eigenschaft für den schreibgeschützten Modus
        public bool ReadOnly
        {
            get { return ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ReadOnly ? true : false; }
            set
            {
                ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ReadOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }

        // Öffentliche Eigenschaft für die Verbindungszeichenfolge
        public string? ConnectionString => ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ConnectionString;

        // Öffentliche Eigenschaft für den Suchbegriff
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

        // Öffentliche Eigenschaft für die CollectionView
        public ICollectionView CollectionView
        {
            get { return _collectionView; }
            set
            {
                _collectionView = value;
                OnPropertyChanged("CollectionView");
            }
        }

        // Öffentliche Eigenschaft für die ausgewählte Runde
        public Runde SelRunde
        {
            get { return _selRunde; }
            set
            {
                _selRunde = value;
                OnPropertyChanged("SelRunde");
            }
        }

        // Öffentliche Eigenschaft für die Liste der Runden
        public ObservableCollection<Runde> LstRunde
        {
            get { return _lstRunde; }
            set
            {
                _lstRunde = value;
                OnPropertyChanged("LstRunde");
            }
        }

        // Öffentliche Eigenschaft für Änderungen
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

        // Öffentliche Eigenschaft für den Ladezustand
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        // Öffentliche Eigenschaft für die Datenbank
        public LaufDBContext Db
        {
            get { return _db; }
            set
            {
                _db = value;
                OnPropertyChanged("Db");
            }
        }

        // Filtermethode für die CollectionView
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

        // Konstruktor, der die Daten lädt
        public RundenseiteModel()
        {
            LoadData();
        }

        // Methode zum Speichern der Änderungen
        public void SaveChanges()
        {
            var currentRunden = Db.Runden.Include(x => x.Laeufer).ToList();
            var removedRunden = currentRunden.Where(cr => !LstRunde.Any(lr => lr.Id == cr.Id)).ToList();
            foreach (var item in removedRunden) Db.Runden.Remove(item);

            Db.SaveChanges();
            HasChanges = false;
            LoadData();
        }

        // Methode zum Laden der Daten
        public void LoadData()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                if (ReadOnly) _db = new(ConnectionString);
                else _db = new();
                LstRunde = new(_db.Runden.Include(x => x.Laeufer).ToList());

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CollectionView = CollectionViewSource.GetDefaultView(LstRunde);
                    CollectionView.Filter = FilterItems;
                    IsLoading = false;
                });
            });
        }
    }
}
