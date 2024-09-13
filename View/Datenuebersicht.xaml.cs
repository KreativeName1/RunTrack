using FullControls.Controls;
using RunTrack.View.Datenuebersicht;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Datenuebersicht.xaml
    /// </summary>
    public partial class Datenuebersicht : Page
    {
        private DatenuebersichtModel _dumodel;
        private PageModel _pmodel;
        public Datenuebersicht()
        {
            InitializeComponent();
            DataContext = this;
            this._dumodel = FindResource("dumodel") as DatenuebersichtModel ?? new DatenuebersichtModel();
            this._pmodel = FindResource("pmodel") as PageModel ?? new PageModel();
            _dumodel.CurrentPage = new Startseite();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            // find the last Scanner page in the history and navigate to it
            Scanner? scanner = _pmodel.History.FindLast(p => p is Scanner) as Scanner;
            _pmodel.Navigate(scanner);
        }

        private void btnStartseite_Click(object sender, RoutedEventArgs e)
        {
            _dumodel.LoadData();
            UebersichtMethoden.CurrentSelectedRow = 0;
            _dumodel.CurrentPage = new Startseite();
            SetButtonState(btnStartseite);
        }

        private void btnRunden_Click(object sender, RoutedEventArgs e)
        {
            _dumodel.LoadData();
            UebersichtMethoden.CurrentSelectedRow = 0;
            _dumodel.CurrentPage = new RundenSeite();
            SetButtonState(btnRunden);
        }

        private void btnSchueler_Click(object sender, RoutedEventArgs e)
        {
            _dumodel.LoadData();
            UebersichtMethoden.CurrentSelectedRow = 0;
            _dumodel.CurrentPage = new SchuelerSeite();
            SetButtonState(btnSchueler);
        }

        private void btnKlassen_Click(object sender, RoutedEventArgs e)
        {
            _dumodel.LoadData();
            UebersichtMethoden.CurrentSelectedRow = 0;
            _dumodel.CurrentPage = new KlassenSeite();
            SetButtonState(btnKlassen);
        }

        private void btnSchule_Click(object sender, RoutedEventArgs e)
        {
            _dumodel.LoadData();
            UebersichtMethoden.CurrentSelectedRow = 0;
            _dumodel.CurrentPage = new SchulenSeite();
            SetButtonState(btnSchule);
        }

        private void btnSchliessen_Click(object sender, RoutedEventArgs e)
        {
            Scanner? scanner = _pmodel.History.FindLast(p => p is Scanner) as Scanner;
            _pmodel.Navigate(scanner);
        }

        private void SetButtonState(ButtonPlus activeButton)
        {
            // Alle Buttons deaktivieren
            btnStartseite.IsEnabled = true;
            btnSchule.IsEnabled = true;
            btnKlassen.IsEnabled = true;
            btnSchueler.IsEnabled = true;
            btnRunden.IsEnabled = true;

            // Den aktuellen Button aktivieren
            activeButton.IsEnabled = false;
        }
    }
}
