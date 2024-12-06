using System.Collections.ObjectModel;

namespace RunTrack
{
    // Modellklasse für den PDF-Editor, die von BaseModel erbt
    public class PDFEditorModel : BaseModel
    {
        // Private Felder für verschiedene Eigenschaften
        private Format? _format;
        private List<Klasse>? _klassen;
        private ObservableCollection<Schueler>? _schueler;
        private bool _neueSeiteProSchueler = true;
        private ObservableCollection<object>? _liste;
        private ObservableCollection<Urkunde>? _urkunden;
        private ObservableCollection<Format>? _formate;
        private ObservableCollection<BlattGroesse>? _blattgroessen;
        private ObservableCollection<SchriftTyp>? schriftTypen;
        private ObservableCollection<Laeufer>? _laeufer;
        private string? _auswertungsArt;
        private Uri? _quelle;

        // Öffentliche Eigenschaften mit Getter und Setter, die OnPropertyChanged aufrufen
        public Format? Format
        {
            get => _format;
            set { _format = value; OnPropertyChanged("Format"); }
        }
        public Uri? Quelle
        {
            get => _quelle;
            set { _quelle = value; OnPropertyChanged("Quelle"); }
        }
        public string? AuswertungsArt
        {
            get => _auswertungsArt;
            set { _auswertungsArt = value; OnPropertyChanged("AuswertungsArt"); }
        }
        public List<Klasse>? Klassen
        {
            get => _klassen;
            set { _klassen = value; OnPropertyChanged("Klasse"); }
        }
        public ObservableCollection<Schueler>? Schueler
        {
            get => _schueler;
            set { _schueler = value; OnPropertyChanged("Schueler"); }
        }
        public ObservableCollection<object>? Liste
        {
            get => _liste;
            set { _liste = value; OnPropertyChanged("Liste"); }
        }
        public ObservableCollection<Urkunde>? Urkunden
        {
            get => _urkunden;
            set { _urkunden = value; OnPropertyChanged("Urkunden"); }
        }
        public bool NeueSeiteProSchueler
        {
            get => _neueSeiteProSchueler;
            set { _neueSeiteProSchueler = value; OnPropertyChanged("NeueSeiteProSchueler"); }
        }

        public ObservableCollection<Format>? Formate
        {
            get => _formate;
            set { _formate = value; OnPropertyChanged("Formate"); }
        }
        public ObservableCollection<BlattGroesse>? Blattgroessen
        {
            get => _blattgroessen;
            set { _blattgroessen = value; OnPropertyChanged("Blattgroessen"); }
        }
        public ObservableCollection<SchriftTyp>? SchriftTypen
        {
            get => schriftTypen;
            set { schriftTypen = value; OnPropertyChanged("SchriftTypen"); }
        }
        public ObservableCollection<Laeufer>? Laeufer
        {
            get => _laeufer;
            set { _laeufer = value; OnPropertyChanged("Laeufer"); }
        }

        // Methode zum Laden der Daten aus der Datenbank
        public void LoadData()
        {
            // Setzt die Listen auf null
            Liste = null;
            Klassen = null;
            Schueler = null;
            Urkunden = null;
            Laeufer = null;
            // Öffnet eine Datenbankverbindung und lädt die Daten
            using (var db = new LaufDBContext())
            {
                Formate = new(db.Formate.ToList());
                Blattgroessen = new(db.BlattGroessen.ToList());
                Format = new() { BlattGroesse = db.BlattGroessen.First(x => x.Name == "A4") };
            }
        }

        // Methode zur Aktualisierung des PDF-Dokuments
        public void AktualisierePDF()
        {
            // Setzt die Quelle auf eine leere Seite
            Quelle = new Uri("about:blank");
            // Generiert das PDF basierend auf den verfügbaren Daten
            if (Klassen != null) Quelle = new Uri(PDFGenerator.BarcodesPDF(Klassen, Format ?? new()));
            else if (Liste != null) Quelle = new Uri(PDFGenerator.AuswertungListe(Liste.ToList(), Format ?? new(), AuswertungsArt ?? string.Empty));
            else if (Urkunden != null) Quelle = new Uri(PDFGenerator.Urkunde(Urkunden.ToList(), Format ?? new()));
            else if (Laeufer != null) Quelle = new Uri(PDFGenerator.BarcodesPDFLaeufer(Laeufer.ToList(), Format ?? new()));
            else Quelle = new Uri(PDFGenerator.SchuelerBewertungPDF(new List<Schueler>(Schueler ?? new()), Format ?? new(), NeueSeiteProSchueler));
        }
    }
}
