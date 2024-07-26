using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Import1.xaml
    /// </summary>
    public partial class Import1 : Page
    {
        private ImportModel? _imodel;
        private int _width = 120;

        public Import1()
        {
            _imodel = FindResource("imodel") as ImportModel ?? new ImportModel();
            InitializeComponent();

            string[] strings = { "Vorname", "Nachname", "Geschlecht", "Geburtsjahrgang", "Klasse" };
            foreach (string s in strings)
            {
                DraggableItem item = new() { TextContent = s, Width = _width };
                OrderPanel.Children.Add(item);
            }

            using (var db = new LaufDBContext())
            {
                ObservableCollection<Schule> schulen = new(db.Schulen.ToList());
                schulen.Insert(0, new Schule { Id = 0, Name = "Neue Schule" });
                _imodel.SchuleListe = schulen;

                // Setze den ersten Eintrag als ausgewählt
                _imodel.Schule = schulen.First();
            }

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

            btnCancel.Click += (s, e) =>
            {
                if (new Popup().Display("Abbrechen", "Möchten Sie den Import wirklich abbrechen?", PopupType.Question, PopupButtons.YesNo).Result == true)
                    _imodel.CloseWindow();
            };
            btnWeiter.Click += (s, e) =>
            {
                if (_imodel.Schule == null || _imodel.Schule.Name == "Neue Schule" && string.IsNullOrWhiteSpace(tbSchule.Text))
                {
                    //MessageBox.Show("Bitte wählen Sie eine Schule aus.");
                    new Popup().Display("Fehler", "Bitte wählen Sie eine Schule aus.", PopupType.Error, PopupButtons.Ok);
                    return;
                }

                _imodel.Reihenfolge = new();
                foreach (DraggableItem item in OrderPanel.Children) _imodel.Reihenfolge.Add(item.TextContent);
                if (_imodel.Schule.Id == 0) _imodel.Schule = new Schule { Name = _imodel.NeuSchuleName };

                // weiter zur klassenerstellung
                _imodel.Import2 = new();
                _imodel.ShowImport2();
            };

            // Subscribe to the LayoutUpdated event
            CSV_Grid.LayoutUpdated += CSV_Grid_LayoutUpdated;
            OrderPanel.LayoutUpdated += (s, e) => UpdateRectangleWidth(); // Update rectangle width on OrderPanel layout update
        }


        private void CSV_Grid_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustColumnWidths();
        }

        private void CSV_Grid_LayoutUpdated(object sender, EventArgs e)
        {
            AdjustColumnWidths();
        }

        private void AdjustColumnWidths()
        {
            double availableWidth = this.ActualWidth - 40; // Adjust as necessary for padding/margins
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
            UpdateRectangleWidth(); // Add this line to update the rectangle width based on the total width of items
        }



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
