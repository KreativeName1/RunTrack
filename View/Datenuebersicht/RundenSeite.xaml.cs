using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack.View.Datenuebersicht
{
    /// <summary>
    /// Interaktionslogik für RundenSeite.xaml
    /// </summary>
    public partial class RundenSeite : Page
    {
        private DatenuebersichtModel _model;
        private LaufDBContext _db = new();
        public RundenSeite()
        {
            InitializeComponent();
            _model = FindResource("dumodel") as DatenuebersichtModel ?? new();

            btnNeu.Click += (sender, e) =>
            {
                Runde neu = new();
                _db.Runden.Add(neu);
                _model.LstRunde.Add(neu);

            };
            btnSpeichern.Click += (sender, e) =>
            {
                _db.SaveChanges();
            };

            btnDel.Click += (sender, e) =>
            {
                Runde runde = lstRunden.SelectedItem as Runde;
                if (runde == null) return;
                _model.LstRunde.Remove(runde);
                Runde dbRunde = _db.Runden.Find(runde.Id);
                if (dbRunde != null)
                {
                    _db.Runden.Remove(dbRunde);
                    _db.SaveChanges();
                }
            };
            lstRunden.CellEditEnding += (sender, e) =>
            {
                Runde runde = lstRunden.SelectedItem as Runde;
                if (runde == null) return;
                Runde dbRunde = _db.Runden.Find(runde.Id);
                if (dbRunde != null)
                {
                    dbRunde.Id = runde.Id;
                    _db.SaveChanges();
                }
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UebersichtMethoden.SearchDataGrid(lstRunden, txtSearch.Text);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstRunden, false, txtSearch.Text);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstRunden, true, txtSearch.Text);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
    }
}
