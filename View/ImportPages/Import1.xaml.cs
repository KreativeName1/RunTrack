using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Import1.xaml
    /// </summary>
    public partial class Import1 : Page
    {
        private ImportModel? _imodel; // ImportModel-Instanz
        private MainModel? _model; // MainModel-Instanz
        private int _width = 120; // Standardbreite für DraggableItems

        // Konstruktor, der den Pfad als Parameter erhält
        public Import1(string pfad)
        {
            // Ressourcen finden oder neue Instanzen erstellen
            _imodel = FindResource("imodel") as ImportModel ?? new ImportModel();
            _model = FindResource("pmodel") as MainModel ?? new MainModel();

            _imodel.Pfad = pfad; // Pfad setzen
            InitializeComponent(); // Komponenten initialisieren

            // Topbar deaktivieren
            if (Application.Current.MainWindow is Main mainWindow)
            {
                mainWindow.SetTopBarEnabled(false);
            }

            // DraggableItems erstellen und zum OrderPanel hinzufügen
            string[] strings = { "Vorname", "Nachname", "Geschlecht", "Geburtsjahrgang", "Klasse" };
            foreach (string s in strings)
            {
                DraggableItem item = new() { TextContent = s, Width = _width };
                OrderPanel.Children.Add(item);
            }

            // Datenbankverbindung herstellen und Schulen laden
            using (var db = new LaufDBContext())
            {
                ObservableCollection<Schule> schulen = new(db.Schulen.ToList());
                schulen.Insert(0, new Schule { Id = 0, Name = "Neue Schule" });
                _imodel.SchuleListe = schulen;

                // Setze den ersten Eintrag als ausgewählt
                _imodel.Schule = schulen.First();
            }

            // CSV-Datei lesen und Fehler behandeln
            try
            {
                _imodel.CSVListe = new(CSVReader.ReadToList(_imodel.Pfad ?? string.Empty));
            }
            catch (FileNotFoundException)
            {
                new Popup().Display("Fehler", "Datei wurde nicht gefunden.", PopupType.Error, PopupButtons.Ok);
                throw new Exception("Schliessen");
            }
            catch (FileLoadException)
            {
                new Popup().Display("Fehler", "Datei konnte nicht geladen werden.", PopupType.Error, PopupButtons.Ok);
                throw new Exception("Schliessen");
            }
            catch (Exception ex)
            {
                new Popup().Display("Fehler", ex.Message, PopupType.Error, PopupButtons.Ok);
                throw new Exception("Schliessen");
            }

            // Event-Handler für Abbrechen-Button
            btnCancel.Click += (s, e) =>
            {
                if (new Popup().Display("Abbrechen", "Möchten Sie den Import wirklich abbrechen?", PopupType.Question, PopupButtons.YesNo) == true)
                {
                    // Letzte Dateiverwaltung finden und navigieren
                    Dateiverwaltung dv = (Dateiverwaltung)_model.History.FindLast(x => x.GetType() == typeof(Dateiverwaltung));
                    DateiVerwaltungModel dvm = FindResource("dvmodel") as DateiVerwaltungModel;
                    if (File.Exists(_imodel.Pfad)) File.Delete(_imodel.Pfad);
                    dvm.LstFiles = new ObservableCollection<FileItem>(FileItem.AlleLesen());
                    _model.Navigate(dv);

                    // Topbar wieder aktivieren
                    if (Application.Current.MainWindow is Main mainWindow)
                    {
                        mainWindow.SetTopBarEnabled(true);
                    }

                    return;
                }
            };

            // Event-Handler für Weiter-Button
            btnWeiter.Click += (s, e) =>
            {
                if (_imodel.Schule == null || (_imodel.Schule.Name == "Neue Schule" && string.IsNullOrWhiteSpace(tbSchule.Text)))
                {
                    new Popup().Display("Fehler", "Bitte wählen Sie eine Schule aus.", PopupType.Error, PopupButtons.Ok);
                    return;
                }

                // Prüfen, ob die neue Schule bereits existiert
                if (_imodel.Schule.Name == "Neue Schule")
                {
                    if (string.Equals(_imodel.NeuSchuleName, "Neue Schule", StringComparison.OrdinalIgnoreCase))
                    {
                        new Popup().Display("Fehler", "Der Name 'Neue Schule' darf nicht verwendet werden. \n\nAndere Kombinationen sind davon möglich", PopupType.Error, PopupButtons.Ok);
                        return;
                    }

                    using (var db = new LaufDBContext())
                    {
                        if (db.Schulen.Any(s => s.Name.ToLower().Trim() == _imodel.NeuSchuleName.ToLower().Trim()))
                        {
                            new Popup().Display("Fehler", "Eine Schule mit diesem Namen existiert bereits.", PopupType.Error, PopupButtons.Ok);
                            return;
                        }
                    }
                }

                // Reihenfolge der DraggableItems speichern
                _imodel.Reihenfolge = new();
                foreach (DraggableItem item in OrderPanel.Children) _imodel.Reihenfolge.Add(item.TextContent);
                if (_imodel.Schule.Id == 0) _imodel.Schule = new Schule { Name = _imodel.NeuSchuleName ?? string.Empty };

                // Weiter zur Klassenerstellung
                _model.Navigate(new Import2());

                

                return;
            };

            // Event-Handler für LayoutUpdated-Event
            CSV_Grid.LayoutUpdated += CSV_Grid_LayoutUpdated;
            OrderPanel.LayoutUpdated += (s, e) => UpdateRectangleWidth(); // Rechteckbreite bei Layout-Update aktualisieren

            // Enter-Taste in tbSchule verhindern
            tbSchule.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                }
            };
        }


        // Event-Handler für das Loaded-Event des CSV_Grid
        private void CSV_Grid_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustColumnWidths();
        }

        // Event-Handler für das LayoutUpdated-Event des CSV_Grid
        private void CSV_Grid_LayoutUpdated(object? sender, EventArgs e)
        {
            AdjustColumnWidths();
        }

        // Methode zur Anpassung der Spaltenbreiten im CSV_Grid
        private void AdjustColumnWidths()
        {
            double availableWidth = this.ActualWidth - 40; // Verfügbare Breite anpassen
            int columnCount = CSV_Grid.Columns.Count;
            if (columnCount > 0)
            {
                double maxColumnWidth = availableWidth / columnCount;
                foreach (var column in CSV_Grid.Columns)
                {
                    column.Width = new DataGridLength(maxColumnWidth, DataGridLengthUnitType.Pixel);
                }
            }

            UpdateOrderPanelWidth();
            UpdateRectangleWidth(); // Rechteckbreite basierend auf der Gesamtbreite der Items aktualisieren
        }

        // Methode zur Aktualisierung der Rechteckbreite
        private void UpdateRectangleWidth()
        {
            double totalWidth = 0;
            double leftPadding = 17;   // Abstand auf der linken Seite
            double rightPadding = 23;  // Abstand auf der rechten Seite

            foreach (DraggableItem item in OrderPanel.Children.OfType<DraggableItem>())
            {
                totalWidth += item.Width;
            }

            // Setze die Breite des Rechtecks basierend auf der Gesamtbreite der Items und den Abständen
            rectBackground.Width = totalWidth + leftPadding + rightPadding;
            rectBackground.Margin = new Thickness(leftPadding, 10, rightPadding, 0); // Setzt die Abstände im Margin
        }

        // Methode zur Aktualisierung der Breite des OrderPanels
        private void UpdateOrderPanelWidth()
        {
            if (CSV_Grid.Columns.Count == OrderPanel.Children.Count)
            {
                for (int i = 0; i < CSV_Grid.Columns.Count; i++)
                {
                    DataGridColumn column = CSV_Grid.Columns[i];
                    if (OrderPanel.Children[i] is DraggableItem item)
                    {
                        item.Width = column.ActualWidth;
                    }
                }
            }
        }
    }
}
