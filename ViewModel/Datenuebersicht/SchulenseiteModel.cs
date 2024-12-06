using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    // Definition der SchulenseiteModel-Klasse, die von BaseModel erbt
    public class SchulenseiteModel : BaseModel
    {
        // Private Felder für die Datenbank, CollectionView, ObservableCollection und andere Eigenschaften
        private LaufDBContext? _db;
        private ICollectionView? _collectionView { get; set; }
        private ObservableCollection<Schule> _lstSchule { get; set; }
        private Schule _selSchule { get; set; }
        private bool _hasChanges { get; set; }
        private bool _isLoading { get; set; }
        private string _searchTerm { get; set; }
        private bool _readOnly { get; set; }

        // Öffentliche Eigenschaft für ReadOnly, die den Wert aus den App-Ressourcen bezieht
        public bool ReadOnly
        {
            get { return ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ReadOnly ? true : false; }
            set
            {
                ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ReadOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }

        // Öffentliche Eigenschaft für ConnectionString, die den Wert aus den App-Ressourcen bezieht
        public string? ConnectionString => ((DatenuebersichtModel)App.Current.Resources["dumodel"]).ConnectionString;

        // Öffentliche Eigenschaft für SearchTerm mit Benachrichtigung bei Änderung
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

        // Öffentliche Eigenschaft für CollectionView mit Benachrichtigung bei Änderung
        public ICollectionView CollectionView
        {
            get { return _collectionView; }
            set
            {
                _collectionView = value;
                OnPropertyChanged("CollectionView");
            }
        }

        // Öffentliche Eigenschaft für LstSchule mit Benachrichtigung bei Änderung
        public ObservableCollection<Schule> LstSchule
        {
            get { return _lstSchule; }
            set
            {
                _lstSchule = value;
                OnPropertyChanged("LstSchule");
            }
        }

        // Öffentliche Eigenschaft für SelSchule mit Benachrichtigung bei Änderung
        public Schule SelSchule
        {
            get { return _selSchule; }
            set
            {
                _selSchule = value;
                OnPropertyChanged("SelSchule");
            }
        }

        // Öffentliche Eigenschaft für HasChanges mit Benachrichtigung bei Änderung
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

        // Öffentliche Eigenschaft für Db mit Benachrichtigung bei Änderung
        public LaufDBContext Db
        {
            get { return _db; }
            set
            {
                _db = value;
                OnPropertyChanged("Db");
            }
        }

        // Öffentliche Eigenschaft für IsLoading mit Benachrichtigung bei Änderung
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        // Filtermethode für die CollectionView
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

        // Konstruktor, der die Daten lädt
        public SchulenseiteModel()
        {
            LoadData();
        }

        // Methode zum Speichern der Änderungen in der Datenbank
        public void SaveChanges()
        {
            var added = LstSchule.ToList().Except(Db.Schulen.AsEnumerable()).ToList();
            var removed = Db.Schulen.AsEnumerable().Except(LstSchule.ToList()).ToList();
            var modified = LstSchule.ToList().Intersect(Db.Schulen.AsEnumerable()).ToList();

            // Führe die notwendigen Aktionen aus
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

            foreach (var schule in LstSchule)
            {
                if (!string.IsNullOrWhiteSpace(schule.Name))
                {
                    schule.Name = CapitalizeWords(schule.Name);
                }
            }

            Db.SaveChanges();

            HasChanges = false;
            LoadData();
        }

        // Methode zur Kapitalisierung der Wörter in einem String
        private string CapitalizeWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }

            return string.Join(' ', words);
        }

        // Methode zur Validierung einer Schule
        public void Validate(Schule schule)
        {
            if (string.IsNullOrEmpty(schule.Name)) throw new ValidationException("Name darf nicht leer sein");
            if (Db.Schulen.Any(s => s.Name.ToLower().Trim() == schule.Name.ToLower().Trim() && s.Id != schule.Id)) throw new ValidationException("Eine Schule mit diesem Namen existiert bereits");
        }

        // Methode zum Laden der Daten
        public void LoadData()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                if (ReadOnly) _db = new(ConnectionString);
                else _db = new();
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
