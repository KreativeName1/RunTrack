using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für SchuelerSeite.xaml
    /// </summary>
    public partial class SchuelerSeite : Page
    {
        private DatenuebersichtModel _model;
        private LaufDBContext _db = new();
        private bool isNewEntry = false;

        public SchuelerSeite()
        {
            InitializeComponent();

            _model = FindResource("dumodel") as DatenuebersichtModel ?? new();
            lstSchueler.PreviewMouseWheel += LstSchueler_PreviewMouseWheel;

            btnNeu.Click += (sender, e) =>
            {
                btnNeu.IsEnabled = false;
                lstSchueler.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                isNewEntry = true;

                int newId = GetNextId();
                Schueler neu = new Schueler { Id = newId, Vorname = "Default", Nachname = "Default" };

                _db.Schueler.Add(neu);
                _model.LstSchueler.Add(neu);

                lstSchueler.ScrollIntoView(neu);
                lstSchueler.SelectedItem = neu;
                lstSchueler.Focus();

                SetEditableForNewRow(neu);
            };

            btnSpeichern.Click += (sender, e) =>
            {
                _db.SaveChanges();
                SetAllRowsEditable();
                lstSchueler.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                isNewEntry = false;
                btnNeu.IsEnabled = true;
            };

            btnDel.Click += (sender, e) =>
            {
                Schueler schueler = lstSchueler.SelectedItem as Schueler;
                if (schueler == null) return;
                _model.LstSchueler.Remove(schueler);
                Schueler dbSchueler = _db.Schueler.Find(schueler.Id);
                if (dbSchueler != null)
                {
                    _db.Schueler.Remove(dbSchueler);
                    _db.SaveChanges();
                }
            };

            lstSchueler.CellEditEnding += (sender, e) =>
            {
                Schueler schueler = lstSchueler.SelectedItem as Schueler;
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
            if (_db.Schueler.Any())
            {
                return _db.Schueler.Max(s => s.Id) + 1;
            }
            return 1;
        }

        private void SetEditableForNewRow(Schueler newRow)
        {
            foreach (var item in lstSchueler.Items)
            {
                if (item != newRow)
                {
                    var row = (DataGridRow)lstSchueler.ItemContainerGenerator.ContainerFromItem(item);
                    if (row != null)
                    {
                        row.IsEnabled = false;
                    }
                }
            }

            var newRowContainer = (DataGridRow)lstSchueler.ItemContainerGenerator.ContainerFromItem(newRow);
            if (newRowContainer != null)
            {
                newRowContainer.IsEnabled = true;
            }
        }

        private void SetAllRowsEditable()
        {
            foreach (var item in lstSchueler.Items)
            {
                var row = (DataGridRow)lstSchueler.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null)
                {
                    row.IsEnabled = true;
                }
            }
        }



        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UebersichtMethoden.SearchDataGrid(lstSchueler, txtSearch.Text);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, false, txtSearch.Text);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, true, txtSearch.Text);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
    }
}
