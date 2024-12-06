using FullControls.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RunTrack
{
    // Hauptfenster der Anwendung, erbt von Page
    public partial class MainWindow : Page
    {
        // Eigenschaften und Felder
        public ResizeMode ResizeMode { get; set; }
        private MainModel _pageModel;
        private DispatcherTimer passwordTimer;

        // Konstruktor
        public MainWindow()
        {
            InitializeComponent();

            // Initialisiere das Modell
            _pageModel = FindResource("pmodel") as MainModel ?? new();

            // Initialisiere den Timer für die Passwortvalidierung
            passwordTimer = new DispatcherTimer();
            passwordTimer.Interval = TimeSpan.FromMilliseconds(500); // 500 ms Inaktivität vor der Validierung
            passwordTimer.Tick += txtPasswort_PasswordTimerTick;

            // Deaktiviere Tab-Stop für den Login-Button
            btnLogin.IsTabStop = false;
        }

        // Event-Handler für das Laden des Fensters
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Setze Fokus und initialisiere Textboxen
            FirstNameTextBox.Focus();
            FirstNameTextBox.ForegroundBrush = new SolidColorBrush(Colors.Gray);
            LastNameTextBox.ForegroundBrush = new SolidColorBrush(Colors.Gray);
            FirstNameTextBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
            LastNameTextBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
            FirstNameTextBox.UnderlineThickness = new Thickness(0, 0, 0, 2);
            LastNameTextBox.UnderlineThickness = new Thickness(0, 0, 0, 2);

            // Prüfen, ob Admin existiert. Wenn nicht, AdminErstellen öffnen
            using (var db = new LaufDBContext())
            {
                if (db.Benutzer.Count() == 0)
                {
                    _pageModel.Navigate(new AdminEinstellungen(DialogMode.Neu));
                    return;
                }
            }
        }

        // Event-Handler für den Login-Button
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Setze Benutzerinformationen
            _pageModel.Benutzer = new Benutzer
            {
                Vorname = FirstNameTextBox.Text,
                Nachname = LastNameTextBox.Text,
            };

            // Überprüfen, ob Vorname und Nachname eingegeben wurden
            if (ValidateInputs())
            {
                using (var db = new LaufDBContext())
                {
                    // Suchen Sie den Benutzer in der Datenbank, Vorname und Nachname werden in Kleinbuchstaben verglichen
                    string eingabeVorname = _pageModel.Benutzer.Vorname.ToLower();
                    string eingabeNachname = _pageModel.Benutzer.Nachname.ToLower();

                    Benutzer? user = db.Benutzer
                        .FirstOrDefault(b => b.Vorname.ToLower() == eingabeVorname && b.Nachname.ToLower() == eingabeNachname);

                    // Wenn der Benutzer nicht gefunden wurde, melden Sie den Benutzer ohne Passwortüberprüfung an
                    if (user == null)
                    {
                        _pageModel.Benutzer.IsAdmin = false; // Oder true, wenn Sie annehmen möchten, dass der Benutzer Administratorrechte hat
                        _pageModel.Navigate(new Scanner());
                    }
                    else
                    {
                        // Wenn der Benutzer existiert, überprüfen Sie das Passwort
                        _pageModel.Benutzer.Passwort = AdminPasswordBox.Password;
                        if (string.IsNullOrEmpty(AdminPasswordBox.Password))
                        {
                            // Kein Passwort eingegeben, anmelden als normaler Benutzer
                            _pageModel.Benutzer.IsAdmin = false; // Setzen Sie auf true, falls der Benutzer Administratorrechte haben soll
                            _pageModel.Navigate(new Scanner());
                        }
                        else if (BCrypt.Net.BCrypt.Verify(AdminPasswordBox.Password, user.Passwort))
                        {
                            // Passwort stimmt überein, anmelden als Administrator
                            _pageModel.Benutzer.IsAdmin = true;
                            _pageModel.Navigate(new Scanner());
                        }
                        else
                        {
                            // Passwort stimmt nicht überein, zeigen Sie eine Warnung an
                            SetInvalidInputStyle(AdminPasswordBox);
                        }
                    }
                }
            }
        }

        // Methode zur Validierung der Eingaben
        private bool ValidateInputs()
        {
            bool isValid = true;

            if (!ValidateFirstName())
            {
                isValid = false;
            }

            if (!ValidateLastName())
            {
                isValid = false;
            }

            // Passwortvalidierung nur durchführen, wenn der Benutzer existiert
            if (gridPasswordLable.Visibility == Visibility.Visible && !ValidatePassword())
            {
                isValid = false;
            }

            return isValid;
        }

        // Methode zur Validierung des Vornamens
        private bool ValidateFirstName()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || FirstNameTextBox.Text == "Max")
            {
                SetInvalidInputStyle(FirstNameTextBox);
                isValid = false;
            }
            else
            {
                SetValidInputStyle(FirstNameTextBox);
            }

            return isValid;
        }

        // Methode zur Validierung des Nachnamens
        private bool ValidateLastName()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) || LastNameTextBox.Text == "Mustermann")
            {
                SetInvalidInputStyle(LastNameTextBox);
                isValid = false;
            }
            else
            {
                SetValidInputStyle(LastNameTextBox);
            }

            return isValid;
        }

        // Methode zur Validierung des Passworts
        private bool ValidatePassword()
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(AdminPasswordBox.Password))
            {
                // Kein Passwort eingegeben, erlauben Sie die Anmeldung als normaler Benutzer, wenn der Benutzer existiert
                using (var db = new LaufDBContext())
                {
                    Benutzer? user = db.Benutzer.FirstOrDefault(b => b.Vorname == FirstNameTextBox.Text.ToLower() && b.Nachname == LastNameTextBox.Text.ToLower());
                    if (user != null)
                    {
                        SetValidInputStyle(AdminPasswordBox);
                        warningPassword.Visibility = Visibility.Hidden; // Kein Passwort nötig, wenn der Benutzer existiert
                    }
                    else
                    {
                        SetInvalidInputStyle(AdminPasswordBox);
                        warningPassword.Visibility = Visibility.Visible; // Warnung anzeigen, wenn der Benutzer nicht existiert
                        isValid = false;
                    }
                }
            }
            else
            {
                // Passwort eingegeben, überprüfen Sie das Passwort
                using (var db = new LaufDBContext())
                {
                    Benutzer? user = db.Benutzer.FirstOrDefault(b => b.Vorname == FirstNameTextBox.Text.ToLower() && b.Nachname == LastNameTextBox.Text.ToLower());
                    if (user != null && BCrypt.Net.BCrypt.Verify(AdminPasswordBox.Password, user.Passwort))
                    {
                        SetValidInputStyle(AdminPasswordBox);
                        warningPassword.Visibility = Visibility.Hidden; // Warnung ausblenden, wenn das Passwort korrekt ist
                    }
                    else
                    {
                        SetInvalidInputStyle(AdminPasswordBox);
                        warningPassword.Visibility = Visibility.Visible; // Warnung anzeigen, wenn das Passwort falsch ist
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        // Methode zum Setzen des Stils für ungültige Eingaben (Passwort)
        private void SetInvalidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush(Colors.Red);
            passwordBox.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
        }

        // Methode zum Setzen des Stils für gültige Eingaben (Passwort)
        private void SetValidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
            passwordBox.UnderlineThickness = new Thickness(0, 0, 0, 2);
        }

        // Methode zum Setzen des Stils für ungültige Eingaben (TextBox)
        private void SetInvalidInputStyle(TextBoxPlus textBox)
        {
            textBox.UnderlineBrush = new SolidColorBrush(Colors.Red);
            textBox.UnderlineThickness = new Thickness(0, 0, 0, 2.5);

            // Null-Überprüfung hinzufügen
            if (textBox.Name == "FirstNameTextBox" && warningVorname != null)
            {
                warningVorname.Visibility = Visibility.Visible;
            }
            else if (textBox.Name == "LastNameTextBox" && warningNachname != null)
            {
                warningNachname.Visibility = Visibility.Visible;
            }
        }

        // Methode zum Setzen des Stils für gültige Eingaben (TextBox)
        private void SetValidInputStyle(TextBoxPlus textBox)
        {
            textBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
            textBox.UnderlineThickness = new Thickness(0, 0, 0, 2);

            // Null-Überprüfung hinzufügen
            if (textBox.Name == "FirstNameTextBox" && warningVorname != null)
            {
                warningVorname.Visibility = Visibility.Hidden;
            }
            else if (textBox.Name == "LastNameTextBox" && warningNachname != null)
            {
                warningNachname.Visibility = Visibility.Hidden;
            }
        }

        // Event-Handler für den Fokus auf die Vorname-Textbox
        private void FirstNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (textBox.Text == "Max")
            {
                textBox.Text = "";
            }
            SetValidInputStyle(textBox);
        }

        // Event-Handler für das Verlassen des Fokus auf die Vorname-Textbox
        private void FirstNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Max";
                textBox.ForegroundBrush = new SolidColorBrush(Colors.Gray);
            }
            ValidateFirstName();
        }

        // Event-Handler für den Fokus auf die Nachname-Textbox
        private void LastNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (textBox.Text == "Mustermann")
            {
                textBox.Text = "";
            }

            SetValidInputStyle(textBox);
        }

        // Event-Handler für das Verlassen des Fokus auf die Nachname-Textbox
        private void LastNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Mustermann";
                textBox.ForegroundBrush = new SolidColorBrush(Colors.Gray);
            }

            ValidateLastName();
        }

        // Event-Handler für das Drücken einer Taste in einer Textbox
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBoxPlus textBox)
                {
                    textBox.ForegroundBrush = new SolidColorBrush(Colors.Blue);
                }
                else if (sender is PasswordBoxPlus passwordBox)
                {
                    passwordBox.Foreground = new SolidColorBrush(Colors.Blue);
                }
                if (ValidateInputs())
                {
                    LoginButton_Click(sender, e);
                }
            }
        }

        // Event-Handler für das Ändern des Textes in der Nachname-Textbox
        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            textBox.ForegroundBrush = new SolidColorBrush(Colors.Blue);

            // Passwortfeld leeren und Sichtbarkeit aktualisieren
            if (AdminPasswordBox != null)
            {
                AdminPasswordBox.Password = string.Empty;
            }

            ValidateLastName();
            UpdateBorderPasswordVisibility();
        }

        // Event-Handler für das Ändern des Textes in der Vorname-Textbox
        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            textBox.ForegroundBrush = new SolidColorBrush(Colors.Blue);

            // Passwortfeld leeren und Sichtbarkeit aktualisieren
            if (AdminPasswordBox != null)
            {
                AdminPasswordBox.Password = string.Empty;
            }

            ValidateFirstName();
            UpdateBorderPasswordVisibility();
        }

        // Methode zur Aktualisierung der Sichtbarkeit des Passwortfelds
        private void UpdateBorderPasswordVisibility()
        {
            if (FirstNameTextBox != null && LastNameTextBox != null && borderPassword != null)
            {
                using (var db = new LaufDBContext())
                {
                    string firstName = FirstNameTextBox.Text.Trim().ToLower();
                    string lastName = LastNameTextBox.Text.Trim().ToLower();

                    Benutzer? user = db.Benutzer
                        .FirstOrDefault(b => b.Vorname.ToLower() == firstName && b.Nachname.ToLower() == lastName);

                    if (user != null)
                    {
                        // Benutzer gefunden, Passwortfeld anzeigen
                        gridPasswordLable.Visibility = Visibility.Visible;
                        borderPassword.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // Benutzer nicht gefunden, Passwortfeld ausblenden
                        gridPasswordLable.Visibility = Visibility.Hidden;
                        borderPassword.Visibility = Visibility.Hidden;
                        warningPassword.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        // Methode zur Aktualisierung der Sichtbarkeit des Passwortfelds
        private void UpdatePasswordFieldVisibility()
        {
            if (FirstNameTextBox != null && LastNameTextBox != null && borderPassword != null)
            {
                using (var db = new LaufDBContext())
                {
                    Benutzer? user = db.Benutzer.FirstOrDefault(b => b.Vorname == FirstNameTextBox.Text && b.Nachname == LastNameTextBox.Text);
                    if (user != null)
                    {
                        gridPasswordLable.Visibility = Visibility.Visible;
                        borderPassword.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        gridPasswordLable.Visibility = Visibility.Hidden;
                        borderPassword.Visibility = Visibility.Hidden;
                        warningPassword.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        // Event-Handler für den Fokus auf das Passwortfeld
        private void txtPasswort_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;
            SetValidInputStyle(passwordBox);
        }

        // Event-Handler für das Verlassen des Fokus auf das Passwortfeld
        private void txtPasswort_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;
            SetValidInputStyle(passwordBox);
        }

        // Event-Handler für das Ändern des Passworts
        private void txtPasswort_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;
            passwordBox.Foreground = new SolidColorBrush(Colors.Blue);

            if (!string.IsNullOrEmpty(passwordBox.Password))
            {
                borderPassword.Visibility = Visibility.Visible;
            }
            else
            {
                // Nur ausblenden, wenn Vorname und Nachname nicht leer und gültig sind
                if (ValidateFirstName() && ValidateLastName())
                {
                    borderPassword.Visibility = Visibility.Visible;
                }
                else
                {
                    borderPassword.Visibility = Visibility.Hidden;
                }
            }

            // Timer zurücksetzen
            if (passwordTimer.IsEnabled)
            {
                passwordTimer.Stop();
            }

            // Timer erneut starten
            passwordTimer.Start();
        }

        // Event-Handler für den Timer-Tick zur Passwortvalidierung
        private async void txtPasswort_PasswordTimerTick(object? sender, EventArgs e)
        {
            // Timer stoppen, um Mehrfachauslösungen zu vermeiden
            passwordTimer.Stop();

            await Task.Run(() =>
            {
                // Passwortvalidierung muss im UI-Thread erfolgen, da auf UI-Komponenten zugegriffen wird
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ValidatePassword();  // Passwortvalidierung im UI-Thread ausführen
                });
            });
        }

        // Event-Handler für das Betreten des Mauszeigers auf das Passwortfeld
        private void AdminPasswordBox_MouseEnter(object sender, MouseEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;

            // Überprüfen, ob das Warnlabel sichtbar ist
            if (warningPassword.Visibility == Visibility.Visible)
            {
                SetInvalidInputStyle(passwordBox);
            }
        }

        // Event-Handler für das Verlassen des Mauszeigers auf das Passwortfeld
        private void AdminPasswordBox_MouseLeave(object sender, MouseEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;

            // Wenn das Passwort gültig ist, setze den gültigen Eingabestil
            if (ValidatePassword())
            {
                SetValidInputStyle(passwordBox);
            }
            else
            {
                SetInvalidInputStyle(passwordBox);
            }
        }

        // Event-Handler für das Klicken auf das Bild (Schließen der Anwendung)
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Event-Handler für das Klicken auf das Bild (Navigieren zu Credits)
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            _pageModel.Navigate(new Credits());
        }
    }
}
