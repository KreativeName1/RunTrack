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
            _model.Db = new LaufDBContext();

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };

            this.btnSpeichern.Click += (s, e) =>
            {
                try
                {
                    _model.SaveChanges();
                }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler beim Speichern", ex.Message, PopupType.Error, PopupButtons.Ok);
                }
            };

            this.btnDel.Click += (s, e) => {

                string message = "";

                if (_model.SelRunde.Id == null)
                {
                    message = "Möchten Sie diesen Eintrag wirklich löschen?";
                }
                else
                {
                    message = $"Möchten Sie diesen Eintrag wirklich löschen? \n- {_model.SelRunde.Id}:\t{_model.SelRunde.Laeufer.Nachname}, {_model.SelRunde.Laeufer.Vorname}";
                }

                var res = new Popup().Display("Löschen", message, PopupType.Question, PopupButtons.YesNo);

                if (res == true)
                {
                    _model.LstRunde.Remove(_model.SelRunde);
                    _model.HasChanges = true;
                }
            };
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstRunden, false);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstRunden, true);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }
    }
}
