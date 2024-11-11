using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack.View.Datenuebersicht
{
    public partial class Startseite : Page
    {
        private StartseiteModel _model;
        public Startseite()
        {
            InitializeComponent();

            _model = FindResource("thismodel") as StartseiteModel;

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
            };
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstStartseite, false);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstStartseite, true);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);

        }
    }
}
