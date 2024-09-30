using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Schulen.xaml
    /// </summary>
    public partial class SchulenSeite : Page
    {
        private DatenuebersichtModel _model;
        private LaufDBContext _db = new();
        public SchulenSeite()
        {
            InitializeComponent();
            _model = FindResource("dumodel") as DatenuebersichtModel ?? new();
            btnNeu.Click += (sender, e) =>
            {
                Schule neu = new();
                _db.Schulen.Add(neu);
                _model.LstSchule.Add(neu);

            };
            btnSpeichern.Click += (sender, e) =>
            {
                _db.SaveChanges();
            };

            btnDel.Click += (sender, e) =>
            {
                Schule schule = lstSchule.SelectedItem as Schule;
                if (schule == null) return;
                _model.LstSchule.Remove(schule);
                Schule dbSchule = _db.Schulen.Find(schule.Id);
                if (dbSchule != null)
                {
                    _db.Schulen.Remove(dbSchule);
                    _db.SaveChanges();
                }
            };

            btnSpeichern.Click += (sender, e) =>
            {
                MessageBox.Show("Speichern");
                using (var db = new LaufDBContext())
                {
                    // update
                    foreach (var schule in _model.LstSchule)
                    {
                        if (_added.Contains(schule)) continue;
                        if (_removed.Contains(schule)) continue;
                        db.Schulen.Update(schule);
                    }
                    // add
                    foreach (var schule in _added)
                    {
                        db.Schulen.Add(schule);
                    }
                    // delete
                    foreach (var schule in _removed)
                    {
                        db.Schulen.Remove(schule);
                    }

                    db.SaveChanges();
                    _added.Clear();
                    _removed.Clear();
                    _model.LoadData();

                }
            };
        }


        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UebersichtMethoden.SearchDataGrid(lstSchule, txtSearch.Text);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchule, false, txtSearch.Text);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchule, true, txtSearch.Text);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
    }
}