using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für SchuelerSeite.xaml
    /// </summary>
    public partial class LaeuferSeite : Page
    {
        private DatenuebersichtModel _model;
        private LaufDBContext _db = new();
        private bool isNewEntry = false;

        public LaeuferSeite()
        {
            InitializeComponent();

            _model = FindResource("dumodel") as DatenuebersichtModel ?? new();
            lstLaeufer.PreviewMouseWheel += LstSchueler_PreviewMouseWheel;

            btnNeu.Click += (sender, e) =>
            {
                btnNeu.IsEnabled = false;
                lstLaeufer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                isNewEntry = true;

                int newId = GetNextId();
                Laeufer neu = new Laeufer { Id = newId, Vorname = "Default", Nachname = "Default" };

                _db.Laeufer.Add(neu);
                _model.LstLaeufer.Add(neu);

                lstLaeufer.ScrollIntoView(neu);
                lstLaeufer.SelectedItem = neu;
                lstLaeufer.Focus();

                SetEditableForNewRow(neu);
            };

            btnSpeichern.Click += (sender, e) =>
            {
                _db.SaveChanges();
                SetAllRowsEditable();
                lstLaeufer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                isNewEntry = false;
                btnNeu.IsEnabled = true;
            };

            btnDel.Click += (sender, e) =>
            {
                Schueler schueler = lstLaeufer.SelectedItem as Schueler;
                if (schueler == null) return;
                _model.LstSchueler.Remove(schueler);
                Schueler dbSchueler = _db.Schueler.Find(schueler.Id);
                if (dbSchueler != null)
                {
                    _db.Schueler.Remove(dbSchueler);
                    _db.SaveChanges();
                }
            };

            lstLaeufer.CellEditEnding += (sender, e) =>
            {
                Schueler schueler = lstLaeufer.SelectedItem as Schueler;
                if (schueler == null) return;
                Schueler dbSchueler = _db.Schueler.Find(schueler.Id);
                if (dbSchueler != null)
                {
                    dbSchueler.Nachname = schueler.Nachname;
                    _db.SaveChanges();
                }
            };
        }

        private void LstSchueler_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (isNewEntry)
            {
                e.Handled = true;  // Suppress the scroll event when adding a new entry
            }
        }

        private int GetNextId()
        {
            if (_db.Laeufer.Any())
            {
                return (int) _db.Laeufer.Max(s => s.Id) + 1;
            }
            return 1;
        }

        private void SetEditableForNewRow(Laeufer newRow)
        {
            foreach (var item in lstLaeufer.Items)
            {
                if (item != newRow)
                {
                    var row = (DataGridRow)lstLaeufer.ItemContainerGenerator.ContainerFromItem(item);
                    if (row != null)
                    {
                        row.IsEnabled = false;
                    }
                }
            }

            var newRowContainer = (DataGridRow)lstLaeufer.ItemContainerGenerator.ContainerFromItem(newRow);
            if (newRowContainer != null)
            {
                newRowContainer.IsEnabled = true;
            }
        }

        private void SetAllRowsEditable()
        {
            foreach (var item in lstLaeufer.Items)
            {
                var row = (DataGridRow)lstLaeufer.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null)
                {
                    row.IsEnabled = true;
                }
            }
        }



        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UebersichtMethoden.SearchDataGrid(lstLaeufer, txtSearch.Text);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstLaeufer, false, txtSearch.Text);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstLaeufer, true, txtSearch.Text);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }


         private void cbRundenArt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_model.SelLaeufer == null)
                return;
            ComboBox checkBox = sender as ComboBox;


            Laeufer laeufer = _db.Laeufer.Find(_model.SelLaeufer.Id);

            RundenArt rundenArt = checkBox.SelectedItem as RundenArt;

            _model.SelRundenArt = rundenArt;

            _model.SelLaeufer.RundenArt = laeufer.RundenArt = rundenArt;

            _db.SaveChanges();
            checkBox.SelectedItem = laeufer.RundenArt;

            _model.SelKlasse = null;
        }
    }
}
