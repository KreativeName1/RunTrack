using FullControls.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für PopupWindow.xaml
    /// </summary>
    public partial class Popup : Window
    {
        private PopupType? _type; // Speichert den Typ des Popups
        private PopupButtons? _buttons; // Speichert die Art der Schaltflächen im Popup
        public bool? Result = null; // Speichert das Ergebnis der Benutzerinteraktion

        // Zeigt das Popup-Fenster an
        public bool? Display(string title, string message, PopupType type, PopupButtons buttons)
        {
            _type = type;
            _buttons = buttons;

            InitializeComponent(); // Initialisiert die Komponenten des Fensters
            Title = title; // Setzt den Titel des Fensters
            tbTitel.Content = title; // Setzt den Titeltext
            tbMessage.Text = message; // Setzt die Nachricht
                                      // Setzt die Höhe des Nachrichtenfeldes abhängig von der Anzahl der Textzeilen

            SetupIcon(); // Richtet das Symbol ein
            SetupButtons(); // Richtet die Schaltflächen ein

            this.Topmost = true; // Setzt das Fenster in den Vordergrund
            this.Activate(); // Aktiviert das Fenster

            this.ShowDialog(); // Zeigt das Fenster als modales Dialogfenster an

            return Result; // Gibt das Ergebnis zurück
        }

        // Richtet das Symbol des Popups basierend auf dem Typ ein
        private void SetupIcon()
        {
            switch (_type)
            {
                case PopupType.Success:
                    imgIcon.Source = new BitmapImage(new Uri(PopupImage.Success.ToString()));
                    break;
                case PopupType.Info:
                    imgIcon.Source = new BitmapImage(new Uri(PopupImage.Info.ToString()));
                    break;
                case PopupType.Warning:
                    imgIcon.Source = new BitmapImage(new Uri(PopupImage.Warning.ToString()));
                    break;
                case PopupType.Error:
                    imgIcon.Source = new BitmapImage(new Uri(PopupImage.Error.ToString()));
                    break;
                case PopupType.Question:
                    imgIcon.Source = new BitmapImage(new Uri(PopupImage.Question.ToString()));
                    break;
            }
        }

        // Richtet die Schaltflächen des Popups basierend auf dem Typ ein
        private void SetupButtons()
        {
            switch (_buttons)
            {
                case PopupButtons.Ok:
                    ButtonPlus button1 = CreateButton("btnOK", "OK");
                    button1.Click += (s, e) =>
                    {
                        Result = true;
                        this.Close();
                    };
                    spButtons.Children.Add(button1);
                    break;
                case PopupButtons.OkCancel:
                    ButtonPlus button2 = CreateButton("btnOK", "OK");
                    button2.Click += (s, e) =>
                    {
                        Result = true;
                        this.Close();
                    };
                    ButtonPlus button3 = CreateButton("btnCancel", "Abbrechen");
                    button3.Click += (s, e) =>
                    {
                        Result = false;
                        this.Close();
                    };
                    spButtons.Children.Add(button2);
                    spButtons.Children.Add(button3);
                    break;
                case PopupButtons.YesNo:
                    ButtonPlus button4 = CreateButton("btnYes", "Ja");
                    button4.Click += (s, e) =>
                    {
                        Result = true;
                        this.Close();
                    };
                    ButtonPlus button5 = CreateButton("btnNo", "Nein");
                    button5.Click += (s, e) =>
                    {
                        Result = false;
                        this.Close();
                    };
                    spButtons.Children.Add(button4);
                    spButtons.Children.Add(button5);
                    break;
                case PopupButtons.YesNoCancel:
                    ButtonPlus button6 = CreateButton("btnYes", "Ja");
                    button6.Click += (s, e) =>
                    {
                        Result = true;
                        this.Close();
                    };
                    ButtonPlus button7 = CreateButton("btnNo", "Nein");
                    button7.Click += (s, e) =>
                    {
                        Result = false;
                        this.Close();
                    };
                    ButtonPlus button8 = CreateButton("btnCancel", "Abbrechen");
                    button8.Click += (s, e) =>
                    {
                        Result = null;
                        this.Close();
                    };
                    spButtons.Children.Add(button6);
                    spButtons.Children.Add(button7);
                    spButtons.Children.Add(button8);
                    break;
            }
        }

        // Erstellt eine Schaltfläche mit den angegebenen Eigenschaften
        private ButtonPlus CreateButton(string name, string content)
        {
            return new ButtonPlus
            {
                Name = name,
                Content = content,
                Width = 80,
                Height = 30,
                Foreground = Brushes.White,
                ForegroundOnMouseOver = Brushes.White,
                BackgroundOnMouseOver = new SolidColorBrush(Color.FromArgb(255, 135, 155, 190)),
                Background = new SolidColorBrush(Color.FromArgb(255, 108, 124, 152)),
                Margin = new Thickness(10, 0, 10, 0)
            };
        }

        // Ereignishandler für das Laden des Fensters
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double height = tbMessage.ActualHeight;
            if (height > 100)
            {
                this.Height += height - 50;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Fenster-Nachrichten abfangen
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_NCLBUTTONDOWN = 0xA1; // Nachricht für Mausklick auf Nicht-Client-Bereich
            const int HTCAPTION = 0x2;         // Titelbereich

            // Bewegung unterbinden
            if (msg == WM_NCLBUTTONDOWN && wParam.ToInt32() == HTCAPTION)
            {
                handled = true; // Blockiere die Nachricht
            }

            return IntPtr.Zero;
        }
    }
}
