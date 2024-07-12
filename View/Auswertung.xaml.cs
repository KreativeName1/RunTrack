using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Auswertung.xaml
    /// </summary>
    public partial class Auswertung : Window
    {
        private string[] _pfade;
        private List<RadioButton> _rundenArten = new List<RadioButton>();
        private AuswertungModel _amodel;
        private MainViewModel _mvmodel;
        public Auswertung()
        {
            _pfade = System.IO.Directory.GetFiles("Dateien", "*.db");
            _amodel = FindResource("amodel") as AuswertungModel;
            _mvmodel = FindResource("mvmodel") as MainViewModel;

            InitializeComponent();


            _amodel.Liste = new ObservableCollection<object>();
            LoadData();
            init();


        }
        public void init()
        {
            btnImport.Click += (s, e) =>
            {
                Dateiverwaltung dateiverwaltung = new Dateiverwaltung();
                dateiverwaltung.Show();

            };
            btnExport.Click += (s, e) =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "files (*.db)|*.db",
                    FileName = "Auswertung.db"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.Copy("internal.db", saveFileDialog.FileName);
                }
            };
            btnSchliessen.Click += (s, e) =>
            {
                Scanner scanner = new Scanner();
                scanner.Show();
                this.Close();
            };
            btnDiagramm.Click += (s, e) =>
            {

            };
            btnWertung.Click += (s, e) =>
            {

            };
            btnSchuelerWertung.Click += (s, e) =>
            {

            };
        }

        public void LoadData()
        {
            using (var db = new MergedDBContext(_pfade))
            {
                bool first = true;
                if (db.RundenArten.Count() == 0) RundenGroesse.Children.Add(new Label { Content = "Keine Rundenarten vorhanden" });
                else foreach (RundenArt rundenArt in db.RundenArten)
                    {
                        RadioButton rb = new RadioButton
                        {
                            Content = rundenArt.Name,
                            Name = rundenArt.Name.Replace(" ", "_"),
                            IsChecked = first
                        };
                        _rundenArten.Add(rb);
                        RundenGroesse.Children.Add(rb);
                        first = false;
                    }

                _amodel.Schulen = new ObservableCollection<Schule>(db.Schulen.ToList());
                _amodel.Klassen = new ObservableCollection<Klasse>(db.Klassen.ToList());
                _amodel.SelectedSchule = _amodel.Schulen.FirstOrDefault();
                _amodel.SelectedKlasse = _amodel.Klassen.FirstOrDefault();


            }
        }

        private void change(object sender, RoutedEventArgs e)
        {
            using (var db = new MergedDBContext(_pfade))
            {

                if (_amodel.IsInsgesamt)
                {
                    _amodel.newList();
                    // get schueler with runden, klasse, klasse->rundenart with intersect/include
                    foreach (Schueler schueler in db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).Where(s => s.Runden.Count() > 1))
                    {
                        string bewertung = GetBewertung(schueler);
                        _amodel.Liste.Add(new { Name = schueler.Vorname + " " + schueler.Nachname, Schule = schueler.Klasse.Schule.Name, Klasse = schueler.Klasse.Name, Bewertung = bewertung });
                    }
                }
                else if (_amodel.IsSchule)
                {
                    _amodel.newList();
                    foreach (Schueler schueler in db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).Where(s => s.Klasse.Schule == _amodel.SelectedSchule && s.Runden.Count() > 1))
                    {
                        string bewertung = GetBewertung(schueler);
                        _amodel.Liste.Add(new { Name = schueler.Vorname + " " + schueler.Nachname, Klasse = schueler.Klasse.Name, Bewertung = bewertung });
                    }
                }
                else if (_amodel.IsKlasse)
                {
                    _amodel.newList();
                    foreach (Schueler schueler in db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).Where(s => s.Klasse == _amodel.SelectedKlasse && s.Runden.Count() > 1))
                    {
                        string bewertung = GetBewertung(schueler);
                        _amodel.Liste.Add(new { Name = schueler.Vorname + " " + schueler.Nachname, Bewertung = bewertung });
                    }
                }
                else if (_amodel.IsJahrgang)
                {
                    _amodel.newList();
                    foreach (Schueler schueler in db.Schueler.Include(s => s.Runden).Include(s => s.Klasse).Where(s => s.Klasse.Jahrgang == _amodel.Jahrgang && s.Runden.Count > 1))
                    {
                        string bewertung = GetBewertung(schueler);
                        _amodel.Liste.Add(new { Name = schueler.Vorname + " " + schueler.Nachname, Klasse = schueler.Klasse.Name, Schule = schueler.Klasse.Schule.Name, Bewertung = bewertung });
                    }
                }
            }
        }

        private void iudJahrgang_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            change(sender, e);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            change(sender, e);
        }

        private string GetBewertung(Schueler schueler)
        {
            string Bewertung = "";
            if (_amodel.IsAnzahl)
            {
                Bewertung = Convert.ToString(schueler.Runden.Where(r => r.Schueler == schueler).Count()-1);
            }
            else if (_amodel.IsZeit)
            {
                List<TimeSpan> rundenZeiten = new List<TimeSpan>();

                for (int i = 1; i < schueler.Runden.Count; i++)
                {
                    TimeSpan rundenzeit = schueler.Runden[i].Zeitstempel - schueler.Runden[i - 1].Zeitstempel;
                    rundenZeiten.Add(rundenzeit);
                }

                // get best time and worst time
                if (rundenZeiten.Count > 0)
                {
                    TimeSpan schnellsteRunde = rundenZeiten[0];
                    TimeSpan langsamsteRunde = rundenZeiten[0];
                    int indexSchnellsteRunde = 0;
                    int indexLangsamsteRunde = 0;
                    TimeSpan gesamtZeit = new TimeSpan(0, 0, 0);

                    for (int i = 1; i < rundenZeiten.Count; i++)
                    {
                        gesamtZeit += rundenZeiten[i];

                        if (rundenZeiten[i] < schnellsteRunde)
                        {
                            schnellsteRunde = rundenZeiten[i];
                            indexSchnellsteRunde = i;
                        }

                        if (rundenZeiten[i] > langsamsteRunde)
                        {
                            langsamsteRunde = rundenZeiten[i];
                            indexLangsamsteRunde = i;
                        }
                    }

                    TimeSpan durchschnittsZeit = new TimeSpan(gesamtZeit.Ticks / rundenZeiten.Count);
                    if (_amodel.IsZeitSchnellste) Bewertung = schnellsteRunde.ToString(@"mm\:ss");
                    else if (_amodel.IsZeitLangsamste) Bewertung = langsamsteRunde.ToString(@"mm\:ss");
                    else if (_amodel.IsZeitDurchschnitt && durchschnittsZeit.Ticks > 0) Bewertung = durchschnittsZeit.ToString(@"mm\:ss");
                    else Bewertung = "--:--";
                }
                else
                {
                    Bewertung = "--:--";
                }
            }
            else if (_amodel.IsDistanz)
            {
                Bewertung = ((schueler.Runden.Count()-1) * schueler.Klasse.RundenArt.LaengeInMeter).ToString("#,##0") + " m";
            }
            return Bewertung;
        }
    }
}
