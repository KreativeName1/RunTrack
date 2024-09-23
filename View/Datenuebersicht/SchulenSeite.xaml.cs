using System.Collections.ObjectModel;
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
        public SchulenSeite()
        {
            InitializeComponent();
            _model = FindResource("dumodel") as DatenuebersichtModel ?? new();
            btnNeu.Click += (sender, e) =>
            {
                using (var db = new LaufDBContext()) {
                    Schule neu = new();
                    db.Schulen.Add(neu);
                    db.SaveChanges();
                    _model.LstSchule.Add(neu);
                }

            };

            btnDel.Click += (sender, e) =>
            {
                using (var db = new LaufDBContext())
                {
                    Schule schule = lstSchule.SelectedItem as Schule;
                    if (schule == null) return;
                    _model.LstSchule.Remove(schule);
                    Schule dbSchule = db.Schulen.Find(schule.Id);
                    if (dbSchule != null)
                    {
                        db.Schulen.Remove(dbSchule);
                        db.SaveChanges();
                    }
                }
            };
            lstSchule.CellEditEnding += (sender, e) =>
            {
                Schule schule = lstSchule.SelectedItem as Schule;
                if (schule == null) return;
                using (var db = new LaufDBContext())
                {
                    Schule dbSchule = db.Schulen.Find(schule.Id);
                    if (dbSchule != null)
                    {
                        dbSchule.Name = schule.Name;
                        db.SaveChanges();
                    }
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
