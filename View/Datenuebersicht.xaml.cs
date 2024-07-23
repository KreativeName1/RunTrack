using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Datenuebersicht.xaml
    /// </summary>
    public partial class Datenuebersicht : Window
    {
        private DatenuebersichtModel _dumodel;
        private MainViewModel _mvmodel;

        private List<DataGridRow> foundRows = new List<DataGridRow>();
        private int currentIndex = -1; // Aktueller Index in der Liste der gefundenen Zeilen

        public static readonly RoutedCommand MyCommand = new RoutedCommand();

        public void CommandExecuted()
        {

        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._dumodel = FindResource("dumodel") as DatenuebersichtModel;
            this._mvmodel = FindResource("mvmodel") as MainViewModel;

            SearchControl.SearchRequested += SearchControl_SearchRequested;

            MyCommand.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));

            LoadData();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchControl.SearchTextBox.Focus();
            // Keyboard.Focus(SearchControl.SearchTextBox);
            // FocusManager.SetFocusedElement(this, SearchControl);
            // SearchControl.BringIntoView();
        }

        [DebuggerStepThrough]
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && SearchControl.SearchTextBox.IsFocused)
            {
                SearchControl_SearchRequested(sender, SearchControl.SearchTextBox.Text);
            }
        }

        private void LstKlasse_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Barcode erstellen Fenster öffnen
            PDFEditor pdfEditor = new PDFEditor(lstKlasse.SelectedItem as Klasse);
            this.Hide();
            _mvmodel.LastWindow = this;
            pdfEditor.Show();
        }

        public Datenuebersicht()
        {
            InitializeComponent();
            DataContext = this;
        }

        private bool SearchAndHighlight(DataGrid dataGrid, string searchText)
        {
            bool found = false;
            foundRows.Clear(); // Vorherige Ergebnisse löschen
            currentIndex = -1; // Index zurücksetzen

            foreach (var item in dataGrid.Items)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null)
                {
                    bool rowFound = false;

                    foreach (var column in dataGrid.Columns)
                    {
                        var cell = column.GetCellContent(item);
                        if (cell != null && cell is TextBlock textBlock)
                        {
                            if (textBlock.Text.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                rowFound = true;
                                found = true;
                                foundRows.Add(row); // Gefundene Zeile zur Liste hinzufügen
                                break;
                            }
                        }
                    }

                    if (rowFound)
                    {
                        row.Background = System.Windows.Media.Brushes.Yellow;
                    }
                    else
                    {
                        row.Background = System.Windows.Media.Brushes.Transparent;
                    }
                }
            }

            if (foundRows.Count > 0)
            {
                currentIndex = 0; // Start bei der ersten gefundenen Zeile
                SelectRow(foundRows[currentIndex]); // Die erste gefundene Zeile auswählen
            }

            return found;
        }

        private void SelectRow(DataGridRow row)
        {
            // Alle Zeilen zurücksetzen
            foreach (var r in foundRows)
            {
                r.Background = System.Windows.Media.Brushes.Yellow;
            }

            // Die ausgewählte Zeile blau markieren
            row.Background = System.Windows.Media.Brushes.Blue;
            // row.Foreground = System.Windows.Media.Brushes.White; // Optional, um den Text lesbar zu machen
            row.Focus(); // Setze den Fokus auf die Zeile
            row.IsSelected = true; // Markiere die Zeile als ausgewählt
        }


        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            GoToNextFound();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousFound();
        }

        private void GoToNextFound()
        {
            if (foundRows.Count == 0 || currentIndex == -1)
                return;

            // Debug-Ausgaben
            Debug.WriteLine($"Current Index: {currentIndex}, Found Rows Count: {foundRows.Count}");

            // Zur nächsten gefundenen Zeile navigieren
            currentIndex = (currentIndex + 1) % foundRows.Count;
            SelectRow(foundRows[currentIndex]);
        }

        private void GoToPreviousFound()
        {
            if (foundRows.Count == 0 || currentIndex == -1)
                return;

            // Debug-Ausgaben
            Debug.WriteLine($"Current Index: {currentIndex}, Found Rows Count: {foundRows.Count}");

            // Zur vorherigen gefundenen Zeile navigieren
            currentIndex = (currentIndex - 1 + foundRows.Count) % foundRows.Count;
            SelectRow(foundRows[currentIndex]);
        }





        // Event-Handler für die Suchanfrage
        private void SearchControl_SearchRequested(object sender, string searchText)
        {
            bool found = false;

            // Suche in allen DataGrids
            if(StartseiteGrid.Visibility == Visibility.Visible)
                found |= SearchAndHighlight(lstStartseite, searchText);

            else if(SchuleGrid.Visibility == Visibility.Visible)
                found |= SearchAndHighlight(lstSchule, searchText);

            else if(KlasseGrid.Visibility == Visibility.Visible)
                found |= SearchAndHighlight(lstKlasse, searchText);

            else if(SchuelerGrid.Visibility == Visibility.Visible)
                found |= SearchAndHighlight(lstSchueler, searchText);

            else if(RundenGrid.Visibility == Visibility.Visible)
                found |= SearchAndHighlight(lstRunden, searchText);

            if (!found)
                MessageBox.Show("Suchtext nicht gefunden.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            // Open admin panel window
            Scanner adminPanel = new Scanner();
            adminPanel.Show();
            this.Close();
        }

        private void btnBarcodes_Click(object sender, RoutedEventArgs e)
        {
            // Logik für Barcode-Button
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            // Logik für Download-Button
        }

        private void btnStartseite_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible);

            using (var db = new LaufDBContext())
            {
                // Lade die Schulen
                _dumodel.LstSchule = new ObservableCollection<Schule>(db.Schulen.ToList());

                // Lade die Klassen mit ihren Rundengrößen
                /*var klassenMitRundengroesse = db.Klassen
                    .Include(k => k.RundenArt)
                    .ToList();

                foreach (var schule in _dumodel.LstSchule)
                {
                    schule.Klassen = new ObservableCollection<Klasse>(klassenMitRundengroesse.Where(k => k.SchuleId == schule.Id));
                }

                // Lade die Schüler
                var schuelerMitKlassen = db.Schueler
                    .Include(s => s.Klasse)
                        .ThenInclude(k => k.RundenArt)
                    .ToList();

                foreach (var schule in _dumodel.LstSchule)
                {
                    foreach (var klasse in schule.Klassen)
                    {
                        klasse.Schueler = new ObservableCollection<Schueler>(schuelerMitKlassen.Where(s => s.KlasseId == klasse.Id));
                    }
                }
                */
            }
        }


        private void btnRunden_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);

            using (var db = new LaufDBContext())
            {
                _dumodel.LstRunde = new ObservableCollection<Runde>(db.Runden.Include(r => r.Schueler).ThenInclude(s => s.Klasse).ThenInclude(k => k.Schule).Include(s => s.Schueler.Klasse).ThenInclude(r => r.RundenArt).ToList());
            }
        }

        private void btnSchueler_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed);

            using (var db = new LaufDBContext())
            {
                _dumodel.LstSchueler = new ObservableCollection<Schueler>(db.Schueler
                    .Include(s => s.Klasse)
                        .ThenInclude(k => k.Schule)
                    .Include(s => s.Klasse)
                        .ThenInclude(r => r.RundenArt)
                    .Include(s => s.Runden)
                    .ToList());
            }
        }

        private void btnKlassen_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);

            using (var db = new LaufDBContext())
            {
                var klassenMitDetails = db.Klassen
                                          .Include(k => k.Schule)
                                          .Include(k => k.Schueler)
                                          .ThenInclude(s => s.Runden)
                                          .ToList();

                _dumodel.LstKlasse = new ObservableCollection<Klasse>(klassenMitDetails);

                if (_dumodel.LstKlasse.Any())
                {
                    var ersteSchule = _dumodel.LstKlasse.First().Schule;
                    if (ersteSchule != null)
                    {
                        var schuleId = ersteSchule.Id;
                        Console.WriteLine($"Die ID der ersten Schule ist: {schuleId}");
                    }
                }

                this.btnBarcodes.Click += (sender, e) =>
                {
                    if (lstKlasse.SelectedItem != null)
                    {
                        PDFEditor pdfEditor = new PDFEditor(lstKlasse.SelectedItem as Klasse);
                        _mvmodel.LastWindow = this;
                        this.Hide();
                        pdfEditor.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Bitte wählen Sie eine Klasse aus!", "Klasse nicht ausgewählt", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                };

            }
        }

        private void btnSchule_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);

            using (var db = new LaufDBContext())
            {
                _dumodel.LstSchule = new ObservableCollection<Schule>(db.Schulen.ToList());
            }
        }

        private void SetVisibility(Visibility schuleGrid, Visibility klasseGrid, Visibility schuelerGrid, Visibility rundenGrid, Visibility startseiteGrid)
        {
            SchuleGrid.Visibility = schuleGrid;
            KlasseGrid.Visibility = klasseGrid;
            SchuelerGrid.Visibility = schuelerGrid;
            RundenGrid.Visibility = rundenGrid;
            StartseiteGrid.Visibility = startseiteGrid;

            btnSchule.Visibility = schuleGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnSchuleDisabled.Visibility = schuleGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnKlassen.Visibility = klasseGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnKlassenDisabled.Visibility = klasseGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnSchueler.Visibility = schuelerGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnSchuelerDisabled.Visibility = schuelerGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnRunden.Visibility = rundenGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnRundenDisabled.Visibility = rundenGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnStartseite.Visibility = startseiteGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnStartseiteDisabled.Visibility = startseiteGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnBarcodes.Visibility = klasseGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadData()
        {
            using (var db = new LaufDBContext())
            {
                _dumodel.LstSchule = new ObservableCollection<Schule>(db.Schulen.ToList());
                _dumodel.LstKlasse = new ObservableCollection<Klasse>(db.Klassen.Include(k => k.Schule).Include(r => r.RundenArt).Include(k => k.Schueler).ThenInclude(s => s.Runden).ToList());
                _dumodel.LstSchueler = new ObservableCollection<Schueler>(db.Schueler.Include(s => s.Klasse).ThenInclude(k => k.Schule).Include(s => s.Runden).ToList());
                _dumodel.LstRunde = new ObservableCollection<Runde>(db.Runden.Include(r => r.Schueler).ThenInclude(s => s.Klasse).ThenInclude(k => k.Schule).ToList());
            }
        }

        public void UpdateSchule(int schuleId, string neuerName)
        {
            using (var db = new LaufDBContext())
            {
                var schule = db.Schueler.FirstOrDefault(s => s.Id == schuleId);
                if (schule != null)
                {
                    schule = db.Schueler.FirstOrDefault(s => s.Id == schuleId);
                    db.SaveChanges();

                    // Überprüfen der Aktualisierung
                    var aktualisierteSchule = db.Schueler.FirstOrDefault(s => s.Id == schuleId);
                    if (aktualisierteSchule != null)
                    {
                        Console.WriteLine("Die Schule wurde erfolgreich aktualisiert.");
                    }
                    else
                    {
                        Console.WriteLine("Fehler beim Aktualisieren der Schule.");
                    }
                }
            }
        }

        private void btnSpeichernAlt_Click(object sender, RoutedEventArgs e)
        {
            if (btnStartseite.Visibility == Visibility.Collapsed)
            {
                using (var db = new LaufDBContext())
                {
                    foreach (Schueler s in lstStartseite.Items)
                    {
                        var schueler = db.Schueler.SingleOrDefault(x => x.Id == s.Id);
                        if (schueler != null)
                        {
                            schueler.Nachname = s.Nachname;
                            schueler.Vorname = s.Vorname;
                            // Weitere Aktualisierungen
                        }
                        else
                        {
                            db.Schueler.Attach(s);
                            db.Entry(s).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
            }
            else if (btnSchule.Visibility == Visibility.Visible)
            {
                // Logik zum Speichern der Schule
            }
            else if (btnKlassen.Visibility == Visibility.Visible)
            {
                // Logik zum Speichern der Klassen
            }
            else if (btnSchueler.Visibility == Visibility.Visible)
            {
                // Logik zum Speichern der Schüler
            }
            else if (btnRunden.Visibility == Visibility.Visible)
            {
                // Logik zum Speichern der Runden
            }
        }
        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (btnStartseite.Visibility == Visibility.Collapsed)
            {
                using (var db = new LaufDBContext())
                {
                    foreach (Schueler s in lstStartseite.Items)
                    {
                        var schueler = db.Schueler.SingleOrDefault(x => x.Id == s.Id);
                        if (schueler != null)
                        {
                            schueler.Nachname = s.Nachname;
                            schueler.Vorname = s.Vorname;
                            // Weitere Aktualisierungen

                            db.Schueler.Update(schueler);
                        }
                    }
                    db.SaveChanges();
                }
            }
            else if (btnSchule.Visibility == Visibility.Visible)
            {
                // Logik zum Speichern der Schule
            }
            else if (btnKlassen.Visibility == Visibility.Visible)
            {
                // Logik zum Speichern der Klassen
            }
            else if (btnSchueler.Visibility == Visibility.Visible)
            {
                // Logik zum Speichern der Schüler
            }
            else if (btnRunden.Visibility == Visibility.Visible)
            {
                // Logik zum Speichern der Runden
            }
        }

        private void btnNeu_Click(object sender, RoutedEventArgs e)
        {
            Schueler s = new Schueler();
            bool saved = false;
            // this.lstSchueler.Items.Add(s);
            while (saved == false)
            {
                saved = Neu(s);

            }
        }

        private bool Neu(Schueler newEntry)
        {
            bool saved = false;

            using (var db = new LaufDBContext())
            {
                //newEntry.Id = _dumodel.LstSchueler.Count +1;
                newEntry.Vorname = "";
                newEntry.Nachname = "";
                newEntry.Geschlecht = Geschlecht.Maennlich;
                //newEntry.Klasse = _dumodel.LstKlasse.First();
                newEntry.Klasse = db.Klassen.Find(_dumodel.LstKlasse.First().Id);

                //_dumodel.LstSchueler.
                int maxId = _dumodel.LstSchueler.Max(x => x.Id);
                _dumodel.LstSchueler.Add(newEntry);

                //lstSchueler.Items.Insert(lstSchueler.Items.Count,newEntry);
                db.Schueler.Add(newEntry);
                db.SaveChanges();

                newEntry = db.Schueler.OrderBy(x => x.Id).Last();

                if (newEntry.Id > maxId)
                {
                    return true;
                }
            }

            return saved;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (lstSchueler.SelectedItem != null)
            {
                Schueler s = lstSchueler.SelectedItem as Schueler;

                using (var db = new LaufDBContext())
                {
                    Schueler delS = db.Schueler.Find(s.Id);
                    db.Schueler.Remove(delS);
                    _dumodel.LstSchueler.Remove(delS);
                    db.SaveChanges();
                };
            }
            else
                MessageBox.Show("Bitte wählen Sie eine Klasse aus!", "Klasse nicht ausgewählt", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        
        /*private void btnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (btnStartseite.Visibility == Visibility.Collapsed)
            {
                foreach (
                    Schueler s in lstStartseite.Items)
                {
                    using (var db = new LaufDBContext())
                    {
                        db.Schueler.Update(s);

                        UpdateSchule(s.Id, s.Nachname);
                    }
                }
            }
            else if (btnSchule.Visibility == Visibility.Visible)
            {

            }
            else if (btnKlassen.Visibility == Visibility.Visible)
            {

            }
            else if (btnSchueler.Visibility == Visibility.Visible)
            {

            }
            else if (btnRunden.Visibility == Visibility.Visible)
            {

            }
        }
        */
    }
}
