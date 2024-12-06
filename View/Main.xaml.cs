using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Main.xaml
    /// </summary>
    public partial class Main : MetroWindow
    {
        private MainModel _pmodel; // Instanz des MainModel
        public Main()
        {
            InitializeComponent(); // Initialisiert die Komponenten
                                   //UniqueKey.DeleteKey(); // Kommentar: Löscht einen eindeutigen Schlüssel
            _pmodel = FindResource("pmodel") as MainModel ?? new(); // Findet die Ressource "pmodel" oder erstellt eine neue Instanz
            MainWindow main = new(); // Erstellt eine neue Instanz von MainWindow
            _pmodel.CurrentPage = main; // Setzt die aktuelle Seite im Modell auf das Hauptfenster

            // Fügt ein Ereignis hinzu, das den Zustand ändert, wenn die F11-Taste gedrückt wird
            this.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.F11) ChangeState();
            };

            // Fügt ein Ereignis hinzu, das auf Größenänderungen des Fensters reagiert
            this.SizeChanged += MetroWindow_SizeChanged;

            // Deaktiviert die Tabstopps für bestimmte Steuerelemente
            BTN_Key.IsTabStop = false;
            ContentFrame.IsTabStop = false;
        }

        // Methode zum Ändern des Fensterzustands (maximiert/normale Größe)
        private void ChangeState()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
                this.IgnoreTaskbarOnMaximize = false;
                this.ShowTitleBar = true;
            }
            else
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                this.IgnoreTaskbarOnMaximize = true;
                this.ShowTitleBar = false;
            }
        }

        // Ereignishandler für Größenänderungen des Fensters
        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                //this.WindowState = WindowState.Normal;

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/maximize.png"));
                img.Width = 20;
                img.Height = 20;

            }
            else
            {
                //this.WindowState = WindowState.Maximized;

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/close_fullscreen.png"));
                img.Width = 15;
                img.Height = 15;
            }
        }

        // Ereignishandler für Klicks auf die Schaltfläche BTN_Key
        private void BTN_Key_Click(object sender, RoutedEventArgs e)
        {
            if (_pmodel?.CurrentPage is SystemKey) return; // Wenn die aktuelle Seite SystemKey ist, nichts tun
            _pmodel?.Navigate(new SystemKey()); // Navigiert zur Seite SystemKey
        }

        // Ereignishandler für das Betreten des Mauszeigers auf die Schaltfläche BTN_Key
        private void BTN_Key_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Key.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#58a394")); // Ändert die Hintergrundfarbe
        }

        // Ereignishandler für das Verlassen des Mauszeigers von der Schaltfläche BTN_Key
        private void BTN_Key_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Key.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007d64")); // Ändert die Hintergrundfarbe
        }

        // Ereignishandler für das Drücken der Maustaste auf die Schaltfläche BTN_Key
        private void BTN_Key_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BTN_Key.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078d7")); // Ändert die Hintergrundfarbe
        }
    }
}
