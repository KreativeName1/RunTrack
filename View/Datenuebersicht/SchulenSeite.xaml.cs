using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Schulen.xaml
    /// </summary>
    public partial class SchulenSeite : Page
    {
        private DatenuebersichtModel _model;
        private ObservableCollection<Schule> _removed = new();
        private ObservableCollection<Schule> _added = new();
        public SchulenSeite()
        {
            InitializeComponent();
            _model = FindResource("dumodel") as DatenuebersichtModel ?? new();
            ObservableCollection<Schule> entities = new(new LaufDBContext().Schulen);
            this.lstSchule.ItemsSource = entities;
            btnNeu.Click += (sender, e) =>
            {
                MessageBox.Show("Neu");
                _added.Add(new Schule());
                _model.LstSchule.Add(new Schule());
            };

            btnDel.Click += (sender, e) =>
            {
                MessageBox.Show("Del");
                if (_model.SelSchule != null)
                {
                    if (_added.Contains(_model.SelSchule as Schule))
                    {
                        _added.Remove(_model.SelSchule as Schule);
                    }
                    else
                    {
                        _removed.Add(_model.SelSchule as Schule);
                    }
                    _model.LstSchule.Remove(_model.SelSchule as Schule);

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
