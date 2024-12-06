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
        public Import3()
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
                    Object page = _model.History.FindLast(x => x.GetType() != typeof(Import1) && x.GetType() != typeof(Import2));
                    _model.Navigate(page);
                }
            };

            btnWeitereDatei.Click += btnWeitereDatei_Click;

            try
            {
                ImportIntoDB importIntoDB = new(_imodel);
                tbTitel.Text = "Erfolg";
                tbMeldung.Text = "Import war erfolgreich.";
                tbTitel.Foreground = Brushes.Green;
                btnSchliessen.Content = "Schließen";
                btnWeitereDatei.Visibility = Visibility.Visible;
            }
            catch (ImportException ex)
            {
                tbTitel.Foreground = Brushes.Red;
                tbTitel.Text = "Fehler";
                tbMeldung.Text = ex.Message;
                btnSchliessen.Content = "Zurück";
            }
            catch (Exception)
            {
                tbTitel.Foreground = Brushes.Red;
                tbTitel.Text = "Fehler";
                tbMeldung.Text = "Ein unerwarteter Fehler ist aufgetreten.";
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
