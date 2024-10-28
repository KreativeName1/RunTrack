using MahApps.Metro.Controls;
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
        private RundenseiteModel _model;
        public RundenSeite()
        {
            InitializeComponent();
            _model = FindResource("thismodel") as RundenseiteModel;

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };
            btnNeu.Click += (s, e) =>
            {
                _model.LstRunde.Add(new Runde());
                _model.HasChanges = true;
            };
            btnSpeichern.Click += (s, e) => { _model.SaveChanges(); };
            btnDel.Click += (s, e) =>
            {
                _model.LstRunde.Remove(_model.SelRunde);
                _model.HasChanges = true;
            };

            lstRunden.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true;
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

        private void DateTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DateTimePicker dateTimePicker = sender as DateTimePicker;

            if (dateTimePicker != null)
            {
                _model.HasChanges = true;
                _model.SelRunde.Zeitstempel = (DateTime)dateTimePicker.SelectedDateTime;
            }
        }
    }
}
