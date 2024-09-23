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

        public SchuelerSeite()
        {
            InitializeComponent();

            _model = FindResource("dumodel") as DatenuebersichtModel ?? new();
            btnNeu.Click += (sender, e) =>
            {
                Schueler neu = new();
                _db.Schueler.Add(neu);
                _model.LstSchueler.Add(neu);

            };
            btnSpeichern.Click += (sender, e) =>
            {
                _db.SaveChanges();
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
