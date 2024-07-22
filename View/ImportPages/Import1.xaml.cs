using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
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
                DraggableItem item = new DraggableItem { TextContent = s, Width = _width };
                OrderPanel.Children.Add(item);
            }

            using (var db = new LaufDBContext())
            {
                ObservableCollection<Schule> schulen = new(db.Schulen.ToList());
                schulen.Insert(0, new Schule { Id = 0, Name = "Neue Schule" });
                _imodel.SchuleListe = schulen;
            }

            try
            {
                _imodel.CSVListe = new(CSVReader.ReadToList(_imodel.Pfad));
            }
            catch (FileNotFoundException) { MessageBox.Show("Datei wurde nicht gefunden."); throw new Exception("Schliessen"); }
            catch (FileLoadException) { MessageBox.Show("Datei konnte nicht geladen werden."); throw new Exception("Schliessen"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); throw new Exception("Schliessen"); }

            btnCancel.Click += (s, e) =>
            {
                if (MessageBox.Show("Möchten Sie den Import wirklich abbrechen?", "Abbrechen", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    _imodel.CloseWindow();
            };
            btnWeiter.Click += (s, e) =>
            {
                if (_imodel.Schule == null || _imodel.Schule.Name == "Neue Schule" && string.IsNullOrWhiteSpace(tbSchule.Text))
                {
                    MessageBox.Show("Bitte wählen Sie eine Schule aus.");
                    return;
                }

                _imodel.Reihenfolge = new();
                foreach (DraggableItem item in OrderPanel.Children) _imodel.Reihenfolge.Add(item.TextContent);
                if (_imodel.Schule.Id == 0) _imodel.Schule = new Schule { Name = _imodel.NeuSchuleName };

                // weiter zur klassenerstellung
                _imodel.Import2 = new();
                _imodel.ShowImport2();
            };

        }

        private void CSV_Grid_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in CSV_Grid.Columns)
            {
                column.Width = new DataGridLength(_width, DataGridLengthUnitType.Pixel);
            }
        }
    }
}
