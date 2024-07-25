using FullControls.Controls;
using Klimalauf.View.Datenuebersicht;
using System.Windows;
using System.Windows.Controls;

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

      // public static readonly RoutedCommand MyCommand = new RoutedCommand();

      //public void CommandExecuted()
      //{

      //}

      //private void Window_Loaded(object sender, RoutedEventArgs e)
      //{
      //    SearchControl.SearchRequested += SearchControl_SearchRequested;

      //    MyCommand.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));

      //    LoadData();
      //}

      //private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
      //{
      //    SearchControl.SearchTextBox.Focus();
      //    // Keyboard.Focus(SearchControl.SearchTextBox);
      //    // FocusManager.SetFocusedElement(this, SearchControl);
      //    // SearchControl.BringIntoView();
      //}

      //[DebuggerStepThrough]
      //private void Window_KeyDown(object sender, KeyEventArgs e)
      //{
      //    if(e.Key == Key.Enter && SearchControl.SearchTextBox.IsFocused)
      //    {
      //        SearchControl_SearchRequested(sender, SearchControl.SearchTextBox.Text);
      //    }
      //}

      //private void LstKlasse_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      //{
      //    // Barcode erstellen Fenster öffnen
      //    PDFEditor pdfEditor = new PDFEditor(lstKlasse.SelectedItem as Klasse ?? new());
      //    this.Hide();
      //    _mvmodel.LastWindow = this;
      //    pdfEditor.Show();
      //}

      public Datenuebersicht()
      {
         InitializeComponent();
         DataContext = this;
         this._dumodel = FindResource("dumodel") as DatenuebersichtModel ?? new DatenuebersichtModel();
         this._mvmodel = FindResource("mvmodel") as MainViewModel ?? new MainViewModel();
         _dumodel.CurrentPage = new Startseite();
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
         //if (foundRows.Count == 0 || currentIndex == -1)
         //    return;

         //// Debug-Ausgaben
         //Debug.WriteLine($"Current Index: {currentIndex}, Found Rows Count: {foundRows.Count}");

         //// Zur nächsten gefundenen Zeile navigieren
         //currentIndex = (currentIndex + 1) % foundRows.Count;
         //SelectRow(foundRows[currentIndex]);
      }

      private void GoToPreviousFound()
      {
         //if (foundRows.Count == 0 || currentIndex == -1)
         //    return;

         //// Debug-Ausgaben
         //Debug.WriteLine($"Current Index: {currentIndex}, Found Rows Count: {foundRows.Count}");

         //// Zur vorherigen gefundenen Zeile navigieren
         //currentIndex = (currentIndex - 1 + foundRows.Count) % foundRows.Count;
         //SelectRow(foundRows[currentIndex]);
      }





      // Event-Handler für die Suchanfrage
      //private void SearchControl_SearchRequested(object sender, string searchText)
      //{
      //    bool found = false;

      //    // Suche in allen DataGrids
      //    if(StartseiteGrid.Visibility == Visibility.Visible)
      //        found |= SearchAndHighlight(lstStartseite, searchText);

      //    else if(SchuleGrid.Visibility == Visibility.Visible)
      //        found |= SearchAndHighlight(lstSchule, searchText);

      //    else if(KlasseGrid.Visibility == Visibility.Visible)
      //        found |= SearchAndHighlight(lstKlasse, searchText);

      //    else if(SchuelerGrid.Visibility == Visibility.Visible)
      //        found |= SearchAndHighlight(lstSchueler, searchText);

      //    else if(RundenGrid.Visibility == Visibility.Visible)
      //        found |= SearchAndHighlight(lstRunden, searchText);

      //    if (!found)
      //        MessageBox.Show("Suchtext nicht gefunden.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
      //}



      private void CloseWindow_Click(object sender, RoutedEventArgs e)
      {
         new Scanner().Show();
         this.Close();
      }

      private void btnStartseite_Click(object sender, RoutedEventArgs e)
      {
         _dumodel.LoadData();
         _dumodel.CurrentPage = new Startseite();
         SetButtonState(btnStartseite);
      }

      private void btnRunden_Click(object sender, RoutedEventArgs e)
      {
         _dumodel.LoadData();
         _dumodel.CurrentPage = new RundenSeite();
         SetButtonState(btnRunden);
      }

      private void btnSchueler_Click(object sender, RoutedEventArgs e)
      {
         _dumodel.LoadData();
         _dumodel.CurrentPage = new SchuelerSeite();
         SetButtonState(btnSchueler);
      }

      private void btnKlassen_Click(object sender, RoutedEventArgs e)
      {
         _dumodel.LoadData();
         _dumodel.CurrentPage = new KlassenSeite();
         SetButtonState(btnKlassen);
      }

      private void btnSchule_Click(object sender, RoutedEventArgs e)
      {
         _dumodel.LoadData();
         _dumodel.CurrentPage = new SchulenSeite();
         SetButtonState(btnSchule);
      }

      private void btnSchliessen_Click(object sender, RoutedEventArgs e)
      {
         new Scanner().Show();
         this.Close();
      }


      private void SetButtonState(ButtonPlus activeButton)
      {
         // Alle Buttons deaktivieren
         btnStartseite.IsEnabled = true;
         btnSchule.IsEnabled = true;
         btnKlassen.IsEnabled = true;
         btnSchueler.IsEnabled = true;
         btnRunden.IsEnabled = true;

         // Den aktuellen Button aktivieren
         activeButton.IsEnabled = false;
      }


      //private void btnSpeichernAlt_Click(object sender, RoutedEventArgs e)
      //{
      //    if (btnStartseite.Visibility == Visibility.Collapsed)
      //    {
      //        using (var db = new LaufDBContext())
      //        {
      //            foreach (Schueler s in lstStartseite.Items)
      //            {
      //                var schueler = db.Schueler.SingleOrDefault(x => x.Id == s.Id);
      //                if (schueler != null)
      //                {
      //                    schueler.Nachname = s.Nachname;
      //                    schueler.Vorname = s.Vorname;
      //                    // Weitere Aktualisierungen
      //                }
      //                else
      //                {
      //                    db.Schueler.Attach(s);
      //                    db.Entry(s).State = EntityState.Modified;
      //                }
      //            }
      //            db.SaveChanges();
      //        }
      //    }
      //    else if (btnSchule.Visibility == Visibility.Visible)
      //    {
      //        // Logik zum Speichern der Schule
      //    }
      //    else if (btnKlassen.Visibility == Visibility.Visible)
      //    {
      //        // Logik zum Speichern der Klassen
      //    }
      //    else if (btnSchueler.Visibility == Visibility.Visible)
      //    {
      //        // Logik zum Speichern der Schüler
      //    }
      //    else if (btnRunden.Visibility == Visibility.Visible)
      //    {
      //        // Logik zum Speichern der Runden
      //    }
      //}
      //private void btnSpeichern_Click(object sender, RoutedEventArgs e)
      //{
      //    if (btnStartseite.Visibility == Visibility.Collapsed)
      //    {
      //        using (var db = new LaufDBContext())
      //        {
      //            foreach (Schueler s in lstStartseite.Items)
      //            {
      //                var schueler = db.Schueler.SingleOrDefault(x => x.Id == s.Id);
      //                if (schueler != null)
      //                {
      //                    schueler.Nachname = s.Nachname;
      //                    schueler.Vorname = s.Vorname;
      //                    // Weitere Aktualisierungen

      //                    db.Schueler.Update(schueler);
      //                }
      //            }
      //            db.SaveChanges();
      //        }
      //    }
      //    else if (btnSchule.Visibility == Visibility.Visible)
      //    {
      //        // Logik zum Speichern der Schule
      //    }
      //    else if (btnKlassen.Visibility == Visibility.Visible)
      //    {
      //        // Logik zum Speichern der Klassen
      //    }
      //    else if (btnSchueler.Visibility == Visibility.Visible)
      //    {
      //        // Logik zum Speichern der Schüler
      //    }
      //    else if (btnRunden.Visibility == Visibility.Visible)
      //    {
      //        // Logik zum Speichern der Runden
      //    }
      //}

      //private void btnNeu_Click(object sender, RoutedEventArgs e)
      //{
      //    Schueler s = new Schueler();
      //    bool saved = false;
      //    // this.lstSchueler.Items.Add(s);
      //    while (saved == false)
      //    {
      //        saved = Neu(s);

      //    }
      //}

      //private bool Neu(Schueler newEntry)
      //{
      //    bool saved = false;

      //    using (var db = new LaufDBContext())
      //    {
      //        //newEntry.Id = _dumodel.LstSchueler.Count +1;
      //        newEntry.Vorname = "";
      //        newEntry.Nachname = "";
      //        newEntry.Geschlecht = Geschlecht.Maennlich;
      //        //newEntry.Klasse = _dumodel.LstKlasse.First();
      //        newEntry.Klasse = db.Klassen.Find(_dumodel.LstKlasse.First().Id);

      //        //_dumodel.LstSchueler.
      //        int maxId = _dumodel.LstSchueler.Max(x => x.Id);
      //        _dumodel.LstSchueler.Add(newEntry);

      //        //lstSchueler.Items.Insert(lstSchueler.Items.Count,newEntry);
      //        db.Schueler.Add(newEntry);
      //        db.SaveChanges();

      //        newEntry = db.Schueler.OrderBy(x => x.Id).Last();

      //        if (newEntry.Id > maxId)
      //        {
      //            return true;
      //        }
      //    }

      //    return saved;
      //}

      //private void btnDel_Click(object sender, RoutedEventArgs e)
      //{
      //    if (lstSchueler.SelectedItem != null)
      //    {
      //        Schueler s = lstSchueler.SelectedItem as Schueler;

      //        using (var db = new LaufDBContext())
      //        {
      //            Schueler delS = db.Schueler.Find(s.Id);
      //            db.Schueler.Remove(delS);
      //            _dumodel.LstSchueler.Remove(delS);
      //            db.SaveChanges();
      //        };
      //    }
      //    else
      //        MessageBox.Show("Bitte wählen Sie eine Klasse aus!", "Klasse nicht ausgewählt", MessageBoxButton.OK, MessageBoxImage.Warning);
      //}
   }
}
