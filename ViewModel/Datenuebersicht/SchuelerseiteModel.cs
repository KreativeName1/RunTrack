using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace RunTrack
{
    public class SchuelerseiteModel : BaseModel
    {
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


        public ObservableCollection<Schueler> LstSchueler
        {
            get { return _lstSchueler; }
            set
            {
                _lstSchueler = value;
                OnPropertyChanged("LstSchueler");
            }
        }

        public Schueler SelSchueler
        {
            get { return _selSchueler; }
            set
            {
                _selSchueler = value;
                OnPropertyChanged("SelSchueler");
            }
        }

        public ObservableCollection<Klasse> LstKlasse
        {
            get { return _lstKlasse; }
            set
            {
                _lstKlasse = value;
                OnPropertyChanged("LstKlasse");
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


        public SchuelerseiteModel()
        {
            LoadData();
        }

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

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            input = input.Trim();
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public void Validate(Schueler schueler)
        {
            if (schueler == null) { throw new ValidationException("Schüler darf nicht leer sein."); }
            if(schueler.Id < 0) { throw new ValidationException("Die Id darf nicht unter 0 sein."); }
            if (schueler.Klasse == null || schueler.Klasse.Id == 0) { throw new ValidationException("Bitte wählen Sie eine Klasse aus."); }
            if (string.IsNullOrEmpty(schueler.Vorname) || string.IsNullOrEmpty(schueler.Nachname) || schueler.Geburtsjahrgang == 0 || schueler.Geschlecht == null) { throw new ValidationException("Es müssen alle Felder ausgefüllt sein."); }
        }

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
