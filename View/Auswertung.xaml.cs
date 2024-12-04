using FullControls.Controls;
using Microsoft.Win32;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Auswertung.xaml
    /// </summary>
    public partial class Auswertung : Page
    {
        private string[] _pfade;
        private List<RadioButtonPlus> _rundenArten = new();
        private AuswertungModel _amodel;
        private MainModel _pmodel;
        private bool _isInitialized = false;
        public Auswertung()
        {
            InitializeComponent();
            if (!System.IO.Directory.Exists("Dateien")) System.IO.Directory.CreateDirectory("Dateien");
            _pfade = System.IO.Directory.GetFiles("Dateien", "*.db");
        }

        public void init()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            btnImport.Click += (s, e) => _pmodel.Navigate(new Dateiverwaltung());
            btnExport.Click += (s, e) =>
            {
                SaveFileDialog saveFileDialog = new() { Filter = "files (*.db)|*.db", FileName = "Auswertung.db" };
                if (saveFileDialog.ShowDialog() == true) System.IO.File.Copy("internal.db", saveFileDialog.FileName);
            };
            btnSchliessen.Click += (s, e) => _pmodel.Navigate(new Scanner());
            btnDiagramm.Click += (s, e) => new Diagramm(_amodel).ShowDialog();
            btnWertung.Click += (s, e) =>
            {
                string auswertungsart = "";
                if (_amodel.IsAnzahl) auswertungsart = "Rundenanzahl";
                else if (_amodel.IsZeit) auswertungsart = "Zeit";
                else if (_amodel.IsDistanz) auswertungsart = "Distanz";
                else auswertungsart = "Rundenanzahl";
                _pmodel.Navigate(new PDFEditor(_amodel.Liste.ToList(), auswertungsart));
            };
            btnSchuelerWertung.Click += (s, e) =>
            {
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
                // Dialog für Auswahl: Alle Werte oder spezifische Werte
                bool? showAllValues = new UrkundePopup().Display("Option wählen", "Möchten Sie alle Werte anzeigen oder spezifische Werte angeben?", PopupType.Question, PopupButtons.YesNoCancel);

                if (showAllValues == null) return; // Abbrechen, falls der Nutzer "Abbrechen" wählt

                // Liste der Objekte
                List<object> liste = _amodel.Liste.ToList();
                int count = Math.Min(liste.Count, 3); // Maximale Anzahl der Einträge begrenzen
                liste = liste.GetRange(0, count);

                // Ermitteln der Auswertungsart
                string auswertungsart = _amodel.IsAnzahl ? "Rundenanzahl" :
                                        _amodel.IsZeit ? "Zeit" :
                                        _amodel.IsDistanz ? "Distanz" : "Rundenanzahl";

                // Ermitteln, worin die Auswertung erfolgt
                string worin = _amodel.IsInsgesamt ? "Insgesamt" :
                               _amodel.IsSchule ? $"Schule {_amodel.SelectedSchule}" :
                               _amodel.IsKlasse ? $"Klasse {_amodel.SelectedKlasse}" :
                               _amodel.IsJahrgang ? $"Jahrgang {_amodel.Jahrgang}" : "";

                // Laufname über ein Eingabepopup erfassen
                InputPopup input = new("Urkunde", "Bitte geben Sie den Namen des Laufes ein");
                input.ShowDialog();
                string laufName = input.GetInputValue<string>();

                if (string.IsNullOrEmpty(laufName) || !input.Result) return; // Abbrechen, falls kein Name eingegeben wurde

                // Urkunden erstellen
                List<Urkunde> urkunden = new();
                foreach (object obj in liste)
                {
                    // Eigenschaften des aktuellen Objekts abrufen
                    string bewertung = obj.GetType().GetProperty("Bewertung")?.GetValue(obj, null)?.ToString() ?? "N/A";
                    string geschlecht = _amodel.IsMaennlich ? "Männlich" :
                                        _amodel.IsWeiblich ? "Weiblich" : "Gesamt";
                    using (var db = new MergedDBContext(_pfade))
                    {
                        int id = Convert.ToInt32(obj.GetType().GetProperty("SchuelerId")?.GetValue(obj, null) ?? 0);
                        Laeufer l = db.Laeufer.First(x => x.Id == id);

                        if (l is Schueler schueler)
                        {

                        }
                        else if (l is Laeufer laeufer)
                        {

                        }
                    }
                    //// Anzahl der Runden, gelaufene Zeit und Distanz (angenommene Eigenschaften)
                    //int anzahlRunden = Convert.ToInt32(obj.GetType().GetProperty("Runden")?.GetValue(obj, null) ?? 0);

                    //// Hier TimeSpan korrekt behandeln, falls null
                    //TimeSpan gelaufeneZeit = obj.GetType().GetProperty("GelaufeneZeit")?.GetValue(obj, null) as TimeSpan? ?? TimeSpan.Zero;

                    //// Distanz ebenfalls behandeln
                    //double distanz = Convert.ToDouble(obj.GetType().GetProperty("Distanz")?.GetValue(obj, null) ?? 0);

                    // Wenn nur spezifische Werte angezeigt werden sollen, diese aus der Objektstruktur extrahieren
                    List<string> specificValues = new List<string>();
                    if (showAllValues == false)
                    {
                        // Nur spezifische Werte wie Name und Bewertung
                        specificValues.Add(obj.GetType().GetProperty("Name")?.GetValue(obj, null)?.ToString() ?? "Unbekannt");
                        specificValues.Add(bewertung); // Füge Bewertung hinzu
                    }

                    // Erstelle die Urkunde
                    urkunden.Add(new Urkunde(
                        laufName,
                        worin,
                        auswertungsart,
                        bewertung,
                        (bool)showAllValues ? null : specificValues, // Alle Werte oder nur spezifische Werte
                        (liste.IndexOf(obj) + 1).ToString(), // Platzierung
                        obj.GetType().GetProperty("Name")?.GetValue(obj, null)?.ToString() ?? "Unbekannt",
                        geschlecht
                    ));
                }

                // Zur PDF-Seite navigieren
                _pmodel.Navigate(new PDFEditor(urkunden));
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
                            Margin = new Thickness(0, 2, 0, 2)
                        };
                        rb.Checked += change;
                        _rundenArten.Add(rb);
                        RundenGroesse.Children.Add(rb);

                        first = false;
                    }
            }
        }

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

                        if (!IsRundenArt(schueler)) continue;

                        if (_amodel.IsMaennlich && schueler.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && schueler.Geschlecht != Geschlecht.Weiblich) continue;
                        if (_amodel.IsDivers && schueler.Geschlecht != Geschlecht.Divers) continue;
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

                        if (!IsRundenArt(schueler)) continue;

                        if (_amodel.IsMaennlich && schueler.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && schueler.Geschlecht != Geschlecht.Weiblich) continue;
                        if (_amodel.IsDivers && schueler.Geschlecht != Geschlecht.Divers) continue;
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

                        if (!IsRundenArt(schueler)) continue;

                        if (_amodel.IsMaennlich && schueler.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && schueler.Geschlecht != Geschlecht.Weiblich) continue;
                        if (_amodel.IsDivers && schueler.Geschlecht != Geschlecht.Divers) continue;
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

                        if (!IsRundenArt(schueler)) continue;

                        if (_amodel.IsMaennlich && schueler.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && schueler.Geschlecht != Geschlecht.Weiblich) continue;
                        if (_amodel.IsDivers && schueler.Geschlecht != Geschlecht.Divers) continue;
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

                        if (!IsRundenArt(laeufer)) continue;

                        if (_amodel.IsMaennlich && laeufer.Geschlecht != Geschlecht.Maennlich) continue;
                        if (_amodel.IsWeiblich && laeufer.Geschlecht != Geschlecht.Weiblich) continue;
                        if (_amodel.IsDivers && laeufer.Geschlecht != Geschlecht.Divers) continue;
                        _amodel.Liste.Add(new { SchuelerId = laeufer.Id, Name = laeufer.Vorname + " " + laeufer.Nachname, Bewertung = bewertung, Geschlecht = geschlecht });

                    }
                }
            }

            if (_amodel.Liste.Count == 0) _amodel.IsLeerListe = true;
            else _amodel.IsLeerListe = false;
        }

        private void iudJahrgang_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            change(sender, e);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            change(sender, e);
        }

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
        private string GetBewertung(Laeufer laeufer)
        {
            if (_amodel.IsAnzahl) return Convert.ToString(laeufer.Runden.Where(r => r.Laeufer == laeufer).Count() - 1) + " Runden";
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
