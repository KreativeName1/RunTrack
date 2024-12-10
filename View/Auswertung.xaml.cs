// Notwendige Namespaces importieren
using FullControls.Controls;
using Microsoft.Win32;
using System.Data.Entity;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Auswertung.xaml
    /// </summary>
    public partial class Auswertung : Page
    {
        // Private Felder für Pfade, Rundenarten, Modelle und Initialisierungsstatus
        private string[] _pfade;
        private List<RadioButtonPlus> _rundenArten = new();
        private AuswertungModel _amodel;
        private MainModel _pmodel;
        private bool _isInitialized = false;

        // Konstruktor der Klasse
        public Auswertung()
        {
            InitializeComponent();
            // Verzeichnis "Dateien" erstellen, falls es nicht existiert
            if (!System.IO.Directory.Exists("Dateien")) System.IO.Directory.CreateDirectory("Dateien");
            // Alle .db-Dateien im Verzeichnis "Dateien" abrufen
            _pfade = System.IO.Directory.GetFiles("Dateien", "*.db");

            


        }

        // Asynchrone Methode zum Kopieren einer Datei
        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            await using FileStream sourceStream = File.Open(sourceFile, FileMode.Open);
            await using FileStream destinationStream = File.Create(destinationFile);
            await sourceStream.CopyToAsync(destinationStream);
        }

        // Initialisierungsmethode
        public void init()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            // Event-Handler für verschiedene Buttons hinzufügen
            btnImport.Click += (s, e) => _pmodel.Navigate(new Dateiverwaltung());
            btnExport.Click += (s, e) =>
            {
                LoadOverlay.Visibility = Visibility.Visible;
                try
                {
                    SaveFileDialog saveFileDialog = new() { Filter = "files (*.db)|*.db", FileName = "Auswertung.db" };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        CopyFileAsync("internal.db", saveFileDialog.FileName);
                    }
                }
                catch (IOException ex)
                {
                    new Popup().Display("Fehler", "Die Datei konnte nicht exportiert werden.", PopupType.Error, PopupButtons.Ok);
                }
            };
            btnSchliessen.Click += (s, e) => _pmodel.Navigate(new Scanner());
            btnDiagramm.Click += (s, e) => new Diagramm(_amodel).ShowDialog();
            btnWertung.Click += (s, e) =>
            {
                LoadOverlay.Visibility = Visibility.Visible;
                string auswertungsart = "";
                if (_amodel.IsAnzahl) auswertungsart = "Rundenanzahl";
                else if (_amodel.IsZeit) auswertungsart = "Zeit";
                else if (_amodel.IsDistanz) auswertungsart = "Distanz";
                else auswertungsart = "Rundenanzahl";
                _pmodel.Navigate(new PDFEditor(_amodel.Liste.ToList(), auswertungsart));
            };
            btnSchuelerWertung.Click += (s, e) =>
            {
                LoadOverlay.Visibility = Visibility.Visible;
                if (_amodel.SelectedItem != null)
                {
                    List<Schueler> schuelerList = new();
                    using (var db = new MergedDBContext(_pfade))
                    {
                        foreach (object? item in Daten.SelectedItems)
                        {
                            if (item == null) continue;
                            if (item.GetType().GetProperty("SchuelerId") == null) continue;
                            int? id = (int?)item.GetType()?.GetProperty("SchuelerId")?.GetValue(item, null);
                            if (id == null) continue;

                            Schueler? schueler = db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).FirstOrDefault(s => s.Id == id);
                            if (schueler != null) schuelerList.Add(schueler);
                        }
                    }
                    _pmodel.Navigate(new PDFEditor(schuelerList));
                }
            };
            btnUrkunde.Click += (s, e) =>
            {
                LoadOverlay.Visibility = Visibility.Visible;

                List<object> liste = _amodel.Liste.ToList();
                liste = liste.GetRange(0, liste.Count);

                string auswertungsart = _amodel.IsAnzahl ? "Rundenanzahl" : (_amodel.IsZeit ? "Zeit" : (_amodel.IsDistanz ? "Distanz" : "Rundenanzahl"));
                string worin = _amodel.IsInsgesamt ? "Insgesamt" : (_amodel.IsSchule ? $"Schule {_amodel.SelectedSchule}" : (_amodel.IsKlasse ? $"Klasse {_amodel.SelectedKlasse}" : $"Jahrgang {_amodel.Jahrgang}"));

                InputPopup input = new("Urkunde", "Bitte geben Sie den Namen des Laufes ein");
                input.Closed += (sender, args) =>
                {
                    LoadOverlay.Visibility = Visibility.Hidden;
                };
                input.ShowDialog();
                string laufName = input.GetInputValue<string>();

                List<Urkunde> urkunden = new();

                foreach (object obj in liste)
                {
                    int? id = obj.GetType().GetProperty("SchuelerId")?.GetValue(obj, null) as int?;
                    if (id == null) continue;

                    using (var db = new MergedDBContext(_pfade))
                    {
                        Schueler? schueler = db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).FirstOrDefault(s => s.Id == id);
                        if (schueler == null) continue;

                        // Berechnung der schnellsten Runde, Rundenanzahl und Meter
                        List<TimeSpan> rundenZeiten = new();
                        for (int i = 1; i < schueler.Runden.Count; i++)
                        {
                            TimeSpan rundenzeit = schueler.Runden[i].Zeitstempel - schueler.Runden[i - 1].Zeitstempel;
                            rundenZeiten.Add(rundenzeit);
                        }

                        TimeSpan schnellsteRunde = rundenZeiten.Count > 0 ? rundenZeiten.Min() : TimeSpan.Zero;
                        int anzahlRunden = rundenZeiten.Count;
                        int gelaufeneMeter = anzahlRunden * schueler.Klasse.RundenArt.LaengeInMeter;
                        string rundenart = schueler.Klasse.RundenArt.Name; // Rundenart holen

                        urkunden.Add(new Urkunde(
                            laufName,
                            auswertungsart,
                            worin,
                            obj.GetType().GetProperty("Bewertung")?.GetValue(obj, null).ToString(),
                            (liste.IndexOf(obj) + 1).ToString(),
                            obj.GetType().GetProperty("Name")?.GetValue(obj, null).ToString(),
                            _amodel.IsMaennlich ? "Männlich" : (_amodel.IsWeiblich ? "Weiblich" : "Gesamt"),
                            schnellsteRunde,
                            anzahlRunden,
                            gelaufeneMeter,
                            rundenart // Rundenart übergeben
                        ));
                    }
                }

                // Sortieren der Urkunden basierend auf der Auswertungsart
                if (auswertungsart == "Rundenanzahl")
                {
                    urkunden = urkunden
                        .OrderByDescending(u => u.AnzahlRunden)
                        .ThenBy(u => u.SchnellsteRunde)
                        .ThenByDescending(u => u.GelaufeneMeter)
                        .ToList();
                }
                else if (auswertungsart == "Zeit")
                {
                    urkunden = urkunden
                        .OrderBy(u => u.SchnellsteRunde)
                        .ThenByDescending(u => u.AnzahlRunden)
                        .ThenByDescending(u => u.GelaufeneMeter)
                        .ToList();
                }
                else if (auswertungsart == "Distanz")
                {
                    urkunden = urkunden
                        .OrderByDescending(u => u.GelaufeneMeter)
                        .ThenByDescending(u => u.AnzahlRunden)
                        .ThenBy(u => u.SchnellsteRunde)
                        .ToList();
                }

                // Platzierung nach der Sortierung aktualisieren
                for (int i = 0; i < urkunden.Count; i++)
                {
                    if (i > 0 && urkunden[i].AnzahlRunden == urkunden[i - 1].AnzahlRunden &&
                        urkunden[i].SchnellsteRunde == urkunden[i - 1].SchnellsteRunde &&
                        urkunden[i].GelaufeneMeter == urkunden[i - 1].GelaufeneMeter)
                    {
                        urkunden[i].Platzierung = urkunden[i - 1].Platzierung;
                    }
                    else
                    {
                        urkunden[i].Platzierung = (i + 1).ToString();
                    }
                }

                if (laufName != null && input.Result) _pmodel.Navigate(new PDFEditor(urkunden));
            };

            btnCSV.Click += (s, e) =>
            {
                SaveFileDialog saveFileDialog = new() { Filter = "files (*.csv)|*.csv", FileName = "Auswertung.csv" };
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var writer = new System.IO.StreamWriter(saveFileDialog.FileName))
                    {
                        writer.WriteLine("Name;Schule;Klasse;Bewertung;Geschlecht");
                        foreach (object item in Daten.SelectedItems)
                        {
                            string name = item.GetType().GetProperty("Name")?.GetValue(item, null).ToString();
                            string schule = item.GetType().GetProperty("Schule")?.GetValue(item, null).ToString();
                            string klasse = item.GetType().GetProperty("Klasse")?.GetValue(item, null).ToString();
                            string bewertung = item.GetType().GetProperty("Bewertung")?.GetValue(item, null).ToString();
                            string geschlecht = item.GetType().GetProperty("Geschlecht")?.GetValue(item, null).ToString();
                            writer.WriteLine($"{name};{schule};{klasse};{bewertung};{geschlecht}");
                        }
                    }
                }
            };
        }

        // Methode zum Laden der Daten
        public void LoadData()
        {
            using (var db = new MergedDBContext(_pfade))
            {
                _amodel.Schulen = new(db.Schulen.ToList());
                _amodel.Klassen = new(db.Klassen.ToList());
                _amodel.SelectedSchule = _amodel.Schulen.FirstOrDefault();
                _amodel.SelectedKlasse = _amodel.Klassen.FirstOrDefault();

                if (_isInitialized) return;

                bool first = true;
                if (db.RundenArten.Count() == 0) RundenGroesse.Children.Add(new Label { Content = "Keine Rundenarten vorhanden" });
                else foreach (RundenArt rundenArt in db.RundenArten)
                    {
                        RadioButtonPlus rb = new()
                        {
                            Content = rundenArt.Name,
                            Name = rundenArt.Name.Replace(" ", "_"),
                            IsChecked = first,
                            Margin = new Thickness(0, 0, 0, 5)
                        };
                        rb.Checked += change;
                        _rundenArten.Add(rb);
                        RundenGroesse.Children.Add(rb);

                        first = false;
                    }
            }
        }

        // Event-Handler für Änderungen
        private void change(object sender, RoutedEventArgs e)
        {
            if (RundenGroesse == null) return;
            using (var db = new MergedDBContext(_pfade))
            {
                if (_amodel.IsInsgesamt)
                {
                    _amodel.Liste = new();
                    foreach (Schueler schueler in db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).Where(s => s.Runden.Count() > 1))
                    {
                        string bewertung = GetBewertung(schueler);
                        string geschlecht = GetGeschlecht(schueler);

                        if (IsRundenArt(schueler)) continue;

                        if (_amodel.IsMaennlich && schueler.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && schueler.Geschlecht != Geschlecht.Weiblich) continue;
                        _amodel.Liste.Add(new { SchuelerId = schueler.Id, Name = schueler.Vorname + " " + schueler.Nachname, Schule = schueler.Klasse.Schule.Name, Klasse = schueler.Klasse.Name, Bewertung = bewertung, Geschlecht = geschlecht });
                    }
                }
                else if (_amodel.IsSchule)
                {
                    _amodel.Liste = new();
                    foreach (Schueler schueler in db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).Where(s => s.Klasse.Schule == _amodel.SelectedSchule && s.Runden.Count() > 1))
                    {
                        string geschlecht = GetGeschlecht(schueler);
                        string bewertung = GetBewertung(schueler);

                        if (IsRundenArt(schueler)) continue;

                        if (_amodel.IsMaennlich && schueler.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && schueler.Geschlecht != Geschlecht.Weiblich) continue;
                        _amodel.Liste.Add(new { SchuelerId = schueler.Id, Name = schueler.Vorname + " " + schueler.Nachname, Klasse = schueler.Klasse.Name, Bewertung = bewertung, Geschlecht = geschlecht });
                    }
                }
                else if (_amodel.IsKlasse)
                {
                    _amodel.Liste = new();
                    foreach (Schueler schueler in db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).Where(s => s.Klasse == _amodel.SelectedKlasse && s.Runden.Count() > 1))
                    {
                        string geschlecht = GetGeschlecht(schueler);
                        string bewertung = GetBewertung(schueler);

                        if (IsRundenArt(schueler)) continue;

                        if (_amodel.IsMaennlich && schueler.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && schueler.Geschlecht != Geschlecht.Weiblich) continue;
                        _amodel.Liste.Add(new { SchuelerId = schueler.Id, Name = schueler.Vorname + " " + schueler.Nachname, Bewertung = bewertung, Geschlecht = geschlecht });
                    }
                }
                else if (_amodel.IsJahrgang)
                {
                    _amodel.Liste = new();
                    foreach (Schueler schueler in db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).Where(s => s.Geburtsjahrgang == _amodel.Jahrgang && s.Runden.Count > 1))
                    {
                        string geschlecht = GetGeschlecht(schueler);
                        string bewertung = GetBewertung(schueler);

                        if (IsRundenArt(schueler)) continue;

                        if (_amodel.IsMaennlich && schueler.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && schueler.Geschlecht != Geschlecht.Weiblich) continue;
                        _amodel.Liste.Add(new { SchuelerId = schueler.Id, Name = schueler.Vorname + " " + schueler.Nachname, Klasse = schueler.Klasse.Name, Schule = schueler.Klasse.Schule.Name, Bewertung = bewertung, Geschlecht = geschlecht });
                    }
                }
                else if (_amodel.IsLaeufer)
                {
                    _amodel.Liste = new();
                    foreach (Laeufer laeufer in db.Laeufer.Where(l => l.RundenArt != null && l.Runden.Count > 1).Include(l => l.Runden).Include(l => l.RundenArt))
                    {
                        string geschlecht = GetGeschlecht(laeufer);
                        string bewertung = GetBewertung(laeufer);

                        if (IsRundenArt(laeufer)) continue;

                        if (_amodel.IsMaennlich && laeufer.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && laeufer.Geschlecht != Geschlecht.Weiblich) continue;
                        _amodel.Liste.Add(new { SchuelerId = laeufer.Id, Name = laeufer.Vorname + " " + laeufer.Nachname, Bewertung = bewertung, Geschlecht = geschlecht });
                    }
                }
            }

            if (_amodel.Liste.Count == 0) _amodel.IsLeerListe = true;
            else _amodel.IsLeerListe = false;
        }

        // Event-Handler für Jahrgangsänderung
        private void iudJahrgang_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            change(sender, e);
        }

        // Event-Handler für Auswahländerung
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            change(sender, e);
        }

        // Methode zur Geschlechtsbestimmung
        private string GetGeschlecht(Laeufer laeufer)
        {
            switch (laeufer.Geschlecht)
            {
                case Geschlecht.Maennlich:
                    return "Männlich";
                case Geschlecht.Weiblich:
                    return "Weiblich";
                case Geschlecht.Divers:
                    return "Divers";
                default:
                    return "";
            }
        }

        // Methode zur Überprüfung der Rundenart
        private bool IsRundenArt(Laeufer laeufer)
        {
            string rundenArtName = string.Empty;
            foreach (RadioButtonPlus rb in RundenGroesse.Children)
            {
                if (rb.IsChecked == true)
                {
                    rundenArtName = rb.Content.ToString() ?? string.Empty;
                    break;
                }
            }
            if (laeufer is Schueler schueler)
            {
                if (!string.IsNullOrWhiteSpace(rundenArtName) && schueler.Klasse.RundenArt.Name != rundenArtName) return true;
            }
            else if (laeufer is Laeufer lauf) if (!string.IsNullOrWhiteSpace(rundenArtName) && lauf.RundenArt.Name != rundenArtName) return true;
            return false;
        }

        // Methode zur Bewertungsermittlung
        private string GetBewertung(Laeufer laeufer)
        {
            int rundenCount = laeufer.Runden.Count() - 1;
            if (_amodel.IsAnzahl)
            {
                return rundenCount == 1 ? "1 Runde  " : $"{rundenCount} Runden";
            }
            else if (_amodel.IsDistanz)
            {
                if (laeufer is Schueler schueler) return ((laeufer.Runden.Count() - 1) * schueler.Klasse.RundenArt.LaengeInMeter).ToString("#,##0") + " m";
                else return ((laeufer.Runden.Count() - 1) * laeufer.RundenArt.LaengeInMeter).ToString("#,##0") + " m";
            }
            else if (_amodel.IsZeit)
            {
                List<TimeSpan> rundenZeiten = new();
                // Alle Timestamps sammeln anhand der Zeitstempel
                for (int i = 1; i < laeufer.Runden.Count; i++)
                {
                    TimeSpan rundenzeit = laeufer.Runden[i].Zeitstempel - laeufer.Runden[i - 1].Zeitstempel;
                    rundenZeiten.Add(rundenzeit);
                }
                return rundenZeiten.Min().ToString(@"mm\:ss") + " min";
            }
            return "--:--";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOverlay.Visibility = Visibility.Hidden;
            _amodel = FindResource("amodel") as AuswertungModel;
            _pmodel = FindResource("pmodel") as MainModel;
            LoadData();
            init();
        }

        private void iudLaeufer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
}
