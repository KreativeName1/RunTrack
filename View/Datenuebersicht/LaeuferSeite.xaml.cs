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
        private LaeuferseiteModel _model;

        public LaeuferSeite()
        {
            InitializeComponent();

            _model = FindResource("thismodel") as LaeuferseiteModel;

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };
            btnNeu.Click += (s, e) =>
            {
                _model.LstLaeufer.Add(new Laeufer());
                _model.HasChanges = true;
            };
            btnSpeichern.Click += (s, e) => { _model.SaveChanges(); };
            btnDel.Click += (s, e) =>
            {
                _model.LstLaeufer.Remove(_model.SelLaeufer);
                _model.HasChanges = true;
            };
            lstLaeufer.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true;
            };
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
            if (_model.SelLaeufer == null) return;
            ComboBox cbRundenArt = sender as ComboBox;
            _model.HasChanges = true;
            RundenArt rundenArt = cbRundenArt.SelectedItem as RundenArt;

            if (rundenArt != null)
            {
                _model.SelLaeufer.RundenArt = rundenArt;
                _model.SelLaeufer.RundenArtId = rundenArt.Id;
            }
        }
    }
}
