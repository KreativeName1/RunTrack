using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    // Definiert das ViewModel für die Schülerseite
    public class SchuelerseiteModel : BaseModel
    {
        // Private Felder für Datenbankkontext, CollectionView und ObservableCollections
        private LaufDBContext? _db;
        private ICollectionView? _collectionView { get; set; }
        private ObservableCollection<Schueler> _lstSchueler { get; set; }
        private ObservableCollection<Schule> _lstSchule { get; set; }
        private ObservableCollection<Klasse> _lstKlasse { get; set; }
        private Schueler _selSchueler { get; set; }
        private bool _hasChanges { get; set; }
        private bool _isLoading { get; set; }
        private string _searchTerm { get; set; }
        private bool _readOnly { get; set; }

        // Eigenschaft für den schreibgeschützten Modus
        public bool ReadOnly
        {
            get { return ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ReadOnly ? true : false; }
            set
            {
                ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ReadOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }

        // Eigenschaft für die Verbindungszeichenfolge
        public string? ConnectionString => ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ConnectionString;

        // Eigenschaft für den Suchbegriff
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

        // Eigenschaft für die CollectionView
        public ICollectionView CollectionView
        {
            get { return _collectionView; }
            set
            {
                _collectionView = value;
                OnPropertyChanged("CollectionView");
            }
        }

        // Eigenschaft für die Liste der Schüler
        public ObservableCollection<Schueler> LstSchueler
        {
            get { return _lstSchueler; }
            set
            {
                _lstSchueler = value;
                OnPropertyChanged("LstSchueler");
            }
        }

        // Eigenschaft für den ausgewählten Schüler
        public Schueler SelSchueler
        {
            get { return _selSchueler; }
            set
            {
                _selSchueler = value;
                OnPropertyChanged("SelSchueler");
            }
        }

        // Eigenschaft für die Liste der Klassen
        public ObservableCollection<Klasse> LstKlasse
        {
            get { return _lstKlasse; }
            set
            {
                _lstKlasse = value;
                OnPropertyChanged("LstKlasse");
            }
        }

        // Eigenschaft für die Liste der Schulen
        public ObservableCollection<Schule> LstSchule
        {
            get { return _lstSchule; }
            set
            {
                _lstSchule = value;
                OnPropertyChanged("LstSchule");
            }
        }

        // Eigenschaft für den Änderungsstatus
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

        // Eigenschaft für den Ladezustand
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        // Eigenschaft für den Datenbankkontext
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
            if (item is Schueler schueler)
            {
                if (string.IsNullOrEmpty(SearchTerm)) return true;
                if (schueler.Id.ToString().Contains(SearchTerm)) return true;
                if (schueler.Vorname.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Nachname.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Geburtsjahrgang.ToString().Contains(SearchTerm)) return true;
                if (schueler.Geschlecht.ToString().ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Klasse.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (schueler.Klasse.Schule.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
            }
            return false;
        }

        // Konstruktor, der die Daten lädt
        public SchuelerseiteModel()
        {
            LoadData();
        }

        // Methode zum Speichern der Änderungen
        public void SaveChanges()
        {
            var added = LstSchueler.ToList().Except(Db.Schueler.AsEnumerable()).ToList();
            var removed = Db.Schueler.AsEnumerable().Except(LstSchueler.ToList()).ToList();
            var modified = LstSchueler.ToList().Intersect(Db.Schueler.AsEnumerable()).ToList();

            foreach (var item in added)
            {
                Validate(item);
                Db.Schueler.Add(item);
            }

            foreach (var item in removed)
            {
                Db.Schueler.Remove(item);
            }

            foreach (var item in modified)
            {
                Validate(item);
                Db.Entry(item).State = EntityState.Modified;
            }

            foreach (var schueler in LstSchueler)
            {
                if (!string.IsNullOrWhiteSpace(schueler.Vorname))
                {
                    schueler.Vorname = CapitalizeFirstLetter(schueler.Vorname);
                    schueler.Nachname = CapitalizeFirstLetter(schueler.Nachname);
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

        // Methode zur Validierung eines Schülers
        public void Validate(Schueler schueler)
        {
            if (schueler == null) { throw new ValidationException("Schüler darf nicht leer sein."); }
            if (schueler.Id < 0) { throw new ValidationException("Die Id darf nicht unter 0 sein."); }
            if (schueler.Klasse == null || schueler.Klasse.Id == 0) { throw new ValidationException("Bitte wählen Sie eine Klasse aus."); }
            if (string.IsNullOrEmpty(schueler.Vorname) || string.IsNullOrEmpty(schueler.Nachname) || schueler.Geburtsjahrgang == 0 || schueler.Geschlecht == null) { throw new ValidationException("Es müssen alle Felder ausgefüllt sein."); }
        }

        // Methode zum Laden der Daten
        public void LoadData()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                if (ReadOnly) _db = new(ConnectionString);
                else _db = new();
                LstSchueler = new(_db.Schueler.Include(s => s.Klasse).ThenInclude(k => k.Schule).Include(s => s.Runden).ToList());
                LstSchule = new(_db.Schulen.ToList());
                LstKlasse = new(_db.Klassen.ToList());

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
