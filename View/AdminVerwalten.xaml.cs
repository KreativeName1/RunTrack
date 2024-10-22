using System.Windows;
using System.Windows.Controls;

namespace RunTrack.View
{
    /// <summary>
    /// Interaktionslogik für AdminVerwalten.xaml
    /// </summary>
    public partial class AdminVerwalten : Page
    {
        private MainModel? _mmodel;
        private AdminModel _admodel;
        private LaufDBContext _db = new();
        public AdminVerwalten()
        {
            InitializeComponent();
            _admodel = FindResource("admodel") as AdminModel ?? new();
            _mmodel = FindResource("pmodel") as MainModel ?? new();

        }

        private void LoadContent()
        {
            //
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            _mmodel.Navigate(new Scanner());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            if (new Popup().Display("Löschen", "Wollen Sie wirklich " + _admodel.SelBenutzer.Nachname + ", " + _admodel.SelBenutzer.Vorname + " löschen?", PopupType.Question, PopupButtons.YesNo) == true)
            {
                Benutzer benutzer = lstAdmins.SelectedItem as Benutzer;
                if (benutzer == null) return;
                this._admodel.LstBenutzer.Remove(benutzer);
                Benutzer dbBenutzer = _db.Benutzer.Find(benutzer.Id);
                if (dbBenutzer != null)
                {
                    _db.Benutzer.Remove(dbBenutzer);
                    _db.SaveChanges();
                }
            }
        }

        private void btnBearbeiten_Click(object sender, RoutedEventArgs e)
        {
            AdminEinstellungen adminEinstellungen = new(DialogMode.Bearbeiten, _admodel.SelBenutzer.Vorname, _admodel.SelBenutzer.Nachname);
            _mmodel.Navigate(adminEinstellungen);
        }

        private void btnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            int history = 1;
            // Überprüfe, ob der letzte Eintrag vom Typ AdminEinstellungen ist
            if (_mmodel.History.Count > 0 && _mmodel.History[^1].GetType() == typeof(AdminEinstellungen))
            {
                history = 2; // Setze den History-Index auf 2, wenn der letzte Eintrag AdminEinstellungen ist

                for (int i = 0; i < _mmodel.History.Count; i++)
                {
                    history++;

                    if (_mmodel.History[^history].GetType() != typeof(AdminVerwalten))
                    {
                        break;
                    }
                    /*if (_mmodel.History[i].GetType() == typeof(AdminEinstellungen))
                    {
                        _mmodel.History.RemoveAt(i);
                    }*/
                }
            }

            // Stelle sicher, dass history innerhalb der Grenzen liegt
            if (_mmodel.History.Count >= history)
            {
                _mmodel?.Navigate(_mmodel.History[^history]); // Navigiere zu dem richtigen History-Element
            }

            LoadContent(); // Lade den Inhalt nach der Navigation
        }

    }

}
