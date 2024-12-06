using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    // Definiert das ViewModel für die Läuferseite
    public class LaeuferseiteModel : BaseModel
    {
        // Private Felder für Datenbankkontext, CollectionView und verschiedene ObservableCollections
        private LaufDBContext? _db;
        private ICollectionView? _collectionView { get; set; }
        private ObservableCollection<Laeufer> _lstLaeufer { get; set; }
        private ObservableCollection<RundenArt> _lstRundenart { get; set; }
        private Laeufer _selLaeufer { get; set; }
        private RundenArt _selRundenArt { get; set; }
        private bool _hasChanges { get; set; }
        private bool _isLoading { get; set; }
        private string _searchTerm { get; set; }
        private ObservableCollection<Laeufer> _selLaeufers { get; set; }
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

        // Eigenschaft für die ausgewählten Läufer
        public ObservableCollection<Laeufer> SelLaeufers
        {
            get { return _selLaeufers; }
            set
            {
                _selLaeufers = value;
                OnPropertyChanged("SelLaeufers");
            }
        }

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

        // Eigenschaft für die Liste der Läufer
        public ObservableCollection<Laeufer> LstLaeufer
        {
            get { return _lstLaeufer; }
            set
            {
                _lstLaeufer = value;
                OnPropertyChanged("LstLaeufer");
            }
        }

        // Eigenschaft für den ausgewählten Läufer
        public Laeufer SelLaeufer
        {
            get { return _selLaeufer; }
            set
            {
                _selLaeufer = value;
                OnPropertyChanged("SelLaeufer");
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

        // Eigenschaft für die Liste der Rundenarten
        public ObservableCollection<RundenArt> LstRundenart
        {
            get { return _lstRundenart; }
            set
            {
                _lstRundenart = value;
                OnPropertyChanged("LstRundenart");
            }
        }

        // Eigenschaft für die ausgewählte Rundenart
        public RundenArt SelRundenArt
        {
            get { return _selRundenArt; }
            set
            {
                _selRundenArt = value;
                OnPropertyChanged("SelRundenArt");
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

        // Methode zum Speichern der Änderungen
        public void SaveChanges()
        {
            var added = LstLaeufer.ToList().Except(Db.Laeufer.AsEnumerable()).ToList();
            var removed = Db.Laeufer.AsEnumerable().Except(LstLaeufer.ToList()).ToList();
            var modified = LstLaeufer.ToList().Intersect(Db.Laeufer.AsEnumerable()).ToList();

            removed.RemoveAll(l => l is Schueler);

            foreach (var item in added)
            {
                Validate(item);
                Db.Laeufer.Add(item);
            }

            foreach (var item in removed)
            {
                Db.Laeufer.Remove(item);
            }

            foreach (var item in modified)
            {
                Validate(item);
                Db.Entry(item).State = EntityState.Modified;
            }

            foreach (var laeufer in LstLaeufer)
            {
                if (!string.IsNullOrWhiteSpace(laeufer.Vorname))
                {
                    laeufer.Vorname = CapitalizeFirstLetter(laeufer.Vorname);
                    laeufer.Nachname = CapitalizeFirstLetter(laeufer.Nachname);
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

        // Filtermethode für die CollectionView
        private bool FilterItems(object item)
        {
            if (item is Laeufer laeufer)
            {
                if (string.IsNullOrEmpty(SearchTerm)) return true;
                if (laeufer.Id.ToString().Contains(SearchTerm)) return true;
                if (laeufer.Vorname.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (laeufer.Nachname.ToLower().Contains(SearchTerm.ToLower())) return true;
                if (laeufer.Geburtsjahrgang.ToString().Contains(SearchTerm)) return true;
                if (laeufer.Geschlecht.ToString().ToLower().Contains(SearchTerm.ToLower())) return true;
                if (laeufer.RundenArt.Name.ToLower().Contains(SearchTerm.ToLower())) return true;
            }
            return false;
        }

        // Konstruktor, der die Daten lädt
        public LaeuferseiteModel()
        {
            LoadData();
        }

        // Methode zur Validierung eines Läufers
        public void Validate(Laeufer laeufer)
        {
            if (laeufer == null) { throw new ValidationException("Läufer darf nicht leer sein."); }
            if (laeufer.RundenArt == null || laeufer.RundenArtId == 0) throw new ValidationException("Rundenart darf nicht leer sein");
            if (laeufer.Geschlecht == null) throw new ValidationException("Geschlecht darf nicht leer sein");
            if (string.IsNullOrWhiteSpace(laeufer.Vorname) || string.IsNullOrWhiteSpace(laeufer.Nachname) || laeufer.Geburtsjahrgang == 0) throw new ValidationException("Alle Felder müssen ausgefüllt werden");
        }

        // Methode zum Laden der Daten
        public void LoadData()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                if (ReadOnly) _db = new(ConnectionString);
                else _db = new();
                LstLaeufer = new(_db.Laeufer.Where(x => x.RundenArt != null));
                LstRundenart = new(_db.RundenArten);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CollectionView = CollectionViewSource.GetDefaultView(LstLaeufer);
                    CollectionView.Filter = FilterItems;
                    IsLoading = false;
                });
            });
        }
    }
}
