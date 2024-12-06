using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Import3.xaml
    /// </summary>
    public partial class Import3 : Page
    {
        private ImportModel _imodel;
        private MainModel _model;
        public Import3(string meldung, bool success)
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new();
            _model = FindResource("pmodel") as MainModel ?? new();
            DataContext = _imodel;

            btnSchliessen.Click += (s, e) =>
            {
                if (tbTitel.Text == "Fehler") _model.Navigate(_model.History[^1]);
                else
                {
                    Object page = _model.History.FindLast(x => x.GetType() == typeof(Dateiverwaltung));
                    _model.Navigate(page);
                }
            };

            btnWeitereDatei.Click += btnWeitereDatei_Click;

            if (success) {
                tbTitel.Text = "Erfolg";
                tbMeldung.Text = meldung;
                tbTitel.Foreground = Brushes.Green;
                btnSchliessen.Content = "Schließen";
                btnWeitereDatei.Visibility = Visibility.Visible;
            }
            else
            {
                tbTitel.Foreground = Brushes.Red;
                tbTitel.Text = "Fehler";
                tbMeldung.Text = meldung;
                btnSchliessen.Content = "Zurück";
            }
        }

        private void btnWeitereDatei_Click(object sender, RoutedEventArgs e)
        {
            _model.Navigate(_model.History.FindLast(x => x.GetType() == typeof(Dateiverwaltung)));
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var dateiverwaltungPage = _model.CurrentPage as Dateiverwaltung;
                dateiverwaltungPage?.ShowRemainingFiles();
            });
        }



    }
}
