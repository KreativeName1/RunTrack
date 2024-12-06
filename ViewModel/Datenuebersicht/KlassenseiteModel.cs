using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    // Definiert das ViewModel für die Klassenseite
    public class KlassenseiteModel : BaseModel
    {
        // Private Felder für den Datenbankkontext und verschiedene ObservableCollections
        private LaufDBContext? _db;
        private ICollectionView? _collectionView { get; set; }
        private ObservableCollection<Klasse> _lstKlasse { get; set; }
        private ObservableCollection<Schule> _lstSchule { get; set; }
        private ObservableCollection<RundenArt> _lstRundenart { get; set; }
        private Klasse _selKlasse { get; set; }
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

        // Öffentliche Eigenschaft für die Liste der Klassen
        public ObservableCollection<Klasse> LstKlasse
        {
            get { return _lstKlasse; }
            set
            {
                _lstKlasse = value;
                OnPropertyChanged("LstKlasse");
            }
        }

        // Öffentliche Eigenschaft für die ausgewählte Klasse
        public Klasse SelKlasse
        {
            get { return _selKlasse; }
            set
            {
                _selKlasse = value;
                OnPropertyChanged("SelKlasse");
            }
        }

        // Öffentliche Eigenschaft für die Liste der Schulen
        public ObservableCollection<Schule> LstSchule
        {
            get { return _lstSchule; }
            set
            {
                _lstSchule = value;
                OnPropertyChanged("LstSchule");
            }
        }

        // Öffentliche Eigenschaft für die Liste der Rundenarten
        public ObservableCollection<RundenArt> LstRundenart
        {
            get { return _lstRundenart; }
            set
            {
                _lstRundenart = value;
                OnPropertyChanged("LstRundenart");
            }
        }

        // Öffentliche Eigenschaft für den Änderungsstatus
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

        // Öffentliche Eigenschaft für den Datenbankkontext
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
            if (item is Klasse klasse)
            {
                if (string.IsNullOrEmpty(SearchTerm)) return true;
                if (klasse.Id.ToString().Contains(SearchTerm)) return true;
                if (klasse.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (klasse.Schule.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (klasse.RundenArt.ToString().ToLower().Contains(SearchTerm)) return true;
            }
            return false;
        }

        // Konstruktor, der die Daten lädt
        public KlassenseiteModel()
        {
            LoadData();
        }

        // Methode zum Speichern der Änderungen
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
                Db.Klassen.Remove(item);
            }

            foreach (var item in modified)
            {
                Validate(item);
                Db.Entry(item).State = EntityState.Modified;
            }

            foreach (var klasse in LstKlasse)
            {
                if (!string.IsNullOrWhiteSpace(klasse.Name))
                {
                    klasse.Name = CapitalizeFirstLetter(klasse.Name);
                }
            }

            Db.SaveChanges();

            HasChanges = false;
            LoadData();
        }

        // Methode zur Kapitalisierung des ersten Buchstabens eines Strings
        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            input = input.Trim();
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        // Methode zur Validierung einer Klasse
        public void Validate(Klasse klasse)
        {
            if (klasse.Schule == null || klasse.Schule.Id == 0) throw new ValidationException("Schule darf nicht leer sein");

            if (klasse.RundenArt == null || klasse.RundenArt.Id == 0) throw new ValidationException("Rundenart darf nicht leer sein");

            if (string.IsNullOrWhiteSpace(klasse.Name)) throw new ValidationException("Name darf nicht leer sein");

            var existingKlasse = Db.Klassen.FirstOrDefault(k =>
                k.Name.Trim().Replace(" ", "").ToLower() == klasse.Name.Trim().Replace(" ", "").ToLower() &&
                k.SchuleId == klasse.SchuleId);

            if (existingKlasse != null && existingKlasse.Id != klasse.Id) throw new ValidationException("Eine Klasse mit diesem Namen existiert bereits in dieser Schule");
        }

        // Methode zum Laden der Daten
        public void LoadData()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                if (ReadOnly) _db = new(ConnectionString);
                else _db = new();
                LstSchule = new(_db.Schulen.ToList());
                LstKlasse = new(_db.Klassen.Include(k => k.Schueler).Include(s => s.Schule).ToList());
                LstRundenart = new(_db.RundenArten.ToList());

                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsLoading = false;
                    CollectionView = CollectionViewSource.GetDefaultView(LstKlasse);
                    CollectionView.Filter = FilterItems;
                });
            });
        }
    }
}
