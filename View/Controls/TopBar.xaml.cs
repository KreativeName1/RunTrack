using System.Windows.Controls;
using System.Windows.Input;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class TopBar : UserControl
    {
        // Private Variable für das MainModel
        private MainModel? _pmodel;

        // Konstruktor für die TopBar-Klasse
        public TopBar()
        {
            InitializeComponent();
            this.DataContext = this; // Setzt den DataContext auf die aktuelle Instanz
            _pmodel = FindResource("pmodel") as MainModel ?? new(); // Initialisiert _pmodel mit einer Ressource oder einer neuen Instanz
        }

        // Event-Handler für das MouseDown-Ereignis des Bildes
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Zeigt eine MessageBox an, um zu bestätigen, ob der Benutzer sich wirklich abmelden möchte
            var result = new Popup().Display("Abmeldung bestätigen", "Möchten Sie sich wirklich abmelden?", PopupType.Question, PopupButtons.OkCancel);

            // Wenn der Benutzer "Ja" auswählt, wird die Abmeldung durchgeführt
            if (result == true)
            {
                _pmodel.Benutzer = null; // Setzt den Benutzer auf null
                _pmodel?.Navigate(new MainWindow()); // Navigiert zur MainWindow-Seite
            }
        }


        // Event-Handler für das MouseDown-Ereignis der Credits
        private void Credits_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_pmodel?.CurrentPage is Credits) return; // Wenn die aktuelle Seite bereits Credits ist, nichts tun
            _pmodel?.Navigate(new Credits()); // Navigiert zur Credits-Seite
        }
    }
}
