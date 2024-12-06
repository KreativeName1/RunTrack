using System.Windows;
using System.Windows.Controls;

namespace RunTrack.View
{
    /// <summary>
    /// Interaktionslogik für AdminVerwalten.xaml
    /// </summary>
    public partial class AdminVerwalten : Page
    {
        private MainModel? _mmodel; // Hauptmodell
        private AdminModel _admodel; // Adminmodell
        private LaufDBContext _db = new(); // Datenbankkontext

        public AdminVerwalten()
        {
            InitializeComponent();
            _admodel = FindResource("admodel") as AdminModel ?? new(); // Adminmodell initialisieren
            _mmodel = FindResource("pmodel") as MainModel ?? new(); // Hauptmodell initialisieren
        }

        private void LoadContent()
        {
            // Inhalt laden
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            _mmodel.Navigate(new Scanner()); // Navigation zu Scanner-Seite
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Speichern-Logik
        }

        private void btnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            // Bestätigungs-Popup anzeigen
            if (new Popup().Display("Löschen", "Wollen Sie wirklich " + _admodel.SelBenutzer.Nachname + ", " + _admodel.SelBenutzer.Vorname + " löschen?", PopupType.Question, PopupButtons.YesNo) == true)
            {
                Benutzer benutzer = lstAdmins.SelectedItem as Benutzer; // Ausgewählten Benutzer abrufen
                if (benutzer == null) return;
                this._admodel.LstBenutzer.Remove(benutzer); // Benutzer aus der Liste entfernen
                Benutzer dbBenutzer = _db.Benutzer.Find(benutzer.Id); // Benutzer aus der Datenbank abrufen
                if (dbBenutzer != null)
                {
                    _db.Benutzer.Remove(dbBenutzer); // Benutzer aus der Datenbank entfernen
                    _db.SaveChanges(); // Änderungen speichern
                }
            }
        }

        private void btnBearbeiten_Click(object sender, RoutedEventArgs e)
        {
            // AdminEinstellungen-Seite mit Bearbeitungsmodus öffnen
            AdminEinstellungen adminEinstellungen = new(DialogMode.Bearbeiten, _admodel.SelBenutzer.Vorname, _admodel.SelBenutzer.Nachname);
            _mmodel.Navigate(adminEinstellungen);
        }

        private void btnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            int history = 1;
            // Überprüfen, ob der letzte Eintrag vom Typ AdminEinstellungen ist
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
                }
            }

            // Sicherstellen, dass history innerhalb der Grenzen liegt
            if (_mmodel.History.Count >= history)
            {
                _mmodel?.Navigate(_mmodel.History[^history]); // Navigation zu dem richtigen History-Element
            }

            LoadContent(); // Inhalt nach der Navigation laden
        }
    }

}
