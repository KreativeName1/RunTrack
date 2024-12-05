using FullControls.Controls;
using RunTrack.View.Datenuebersicht;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Datenuebersicht.xaml
    /// </summary>
    public partial class Datenuebersicht : Page
    {
        private DatenuebersichtModel _dumodel;
        private MainModel _pmodel;
        public Datenuebersicht()
        {
            InitializeComponent();
            DataContext = this;
            this._dumodel = FindResource("dumodel") as DatenuebersichtModel ?? new DatenuebersichtModel();
            this._pmodel = FindResource("pmodel") as MainModel ?? new MainModel();
            changePage(new Startseite(), btnStartseite);

            btnStartseite.Click += (s, e) => changePage(new Startseite(), s as ButtonPlus);
            btnSchule.Click += (s, e) => changePage(new SchulenSeite(), s as ButtonPlus);
            btnSchueler.Click += (s, e) => changePage(new SchuelerSeite(), s as ButtonPlus);
            btnLaeufer.Click += (s, e) => changePage(new LaeuferSeite(), s as ButtonPlus);
            btnKlassen.Click += (s, e) => changePage(new KlassenSeite(), s as ButtonPlus);
            btnRunden.Click += (s, e) => changePage(new RundenSeite(), s as ButtonPlus);

            btnSchliessen.Click += (s, e) => _pmodel.Navigate(_pmodel.History.FindLast(p => p is Scanner));
        }
        private void changePage(Page page, ButtonPlus button)
        {
            if (_dumodel.HasChanges)
            {
                bool? result = new Popup().Display("Änderungen verwerfen?", "Es gibt ungespeicherte Änderungen. Wirklich fortfahren?", PopupType.Question, PopupButtons.YesNo);
                if (result == true) _dumodel.HasChanges = false;
                else if (result == false) return;
            }
            _dumodel.HasChanges = false;
            _dumodel.CurrentPage = page;
            UebersichtMethoden.CurrentSelectedRow = 0;

            btnStartseite.IsEnabled = true;
            btnSchule.IsEnabled = true;
            btnKlassen.IsEnabled = true;
            btnSchueler.IsEnabled = true;
            btnRunden.IsEnabled = true;
            btnLaeufer.IsEnabled = true;

            button.IsEnabled = false;
        }

        // NUR FÜR DEBUGGING
        //private void btnTest_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    _dumodel.ReadOnly = !_dumodel.ReadOnly;
        //}
    }
}
