using FullControls.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RunTrack
{
    public partial class MainWindow : Page
    {
        public ResizeMode ResizeMode { get; set; }
        private MainModel _pageModel;
        private DispatcherTimer passwordTimer;

        public MainWindow()
        {
            InitializeComponent();

            _pageModel = FindResource("pmodel") as MainModel ?? new();


            passwordTimer = new DispatcherTimer();
            passwordTimer.Interval = TimeSpan.FromMilliseconds(500); // 500 ms of inactivity before validation
            passwordTimer.Tick += txtPasswort_PasswordTimerTick;

            btnLogin.IsTabStop = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void SetInvalidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush(Colors.Red);
            passwordBox.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
        }

        private void SetValidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
            passwordBox.UnderlineThickness = new Thickness(0, 0, 0, 2);
        }

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


        private void FirstNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (textBox.Text == "Max")
            {
                textBox.Text = "";
            }
            SetValidInputStyle(textBox);
        }

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

        private void LastNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (textBox.Text == "Mustermann")
            {
                textBox.Text = "";
            }

            SetValidInputStyle(textBox);
        }

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

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            textBox.ForegroundBrush = new SolidColorBrush(Colors.Blue);

            // Clear the password box and update the border visibility
            if (AdminPasswordBox != null)
            {
                AdminPasswordBox.Password = string.Empty;
            }

            ValidateLastName();
            UpdateBorderPasswordVisibility();
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            textBox.ForegroundBrush = new SolidColorBrush(Colors.Blue);

            // Clear the password box and update the border visibility
            if (AdminPasswordBox != null)
            {
                AdminPasswordBox.Password = string.Empty;
            }

            ValidateFirstName();
            UpdateBorderPasswordVisibility();
        }

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

        private void txtPasswort_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;
            SetValidInputStyle(passwordBox);
        }

        private void txtPasswort_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;
            SetValidInputStyle(passwordBox);
        }

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
                // Only collapse if FirstName and LastName are not empty and valid
                if (ValidateFirstName() && ValidateLastName())
                {
                    borderPassword.Visibility = Visibility.Visible;
                }
                else
                {
                    borderPassword.Visibility = Visibility.Hidden;
                }
            }

            // Reset the timer
            if (passwordTimer.IsEnabled)
            {
                passwordTimer.Stop();
            }

            // Start the timer again
            passwordTimer.Start();
        }

        // ValidatePassword() takes between 223 and 589 milliseconds to execute. 
        // This is primarily because BCrypt.Net.BCrypt.Verify, which is used for password verification, is a time-consuming operation.
        private async void txtPasswort_PasswordTimerTick(object? sender, EventArgs e)
        {
            // Stop the timer to avoid multiple triggers
            passwordTimer.Stop();

            await Task.Run(() =>
            {
                // ValidatePassword needs to be invoked on the UI thread because it accesses UI components.
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ValidatePassword();  // Run password validation on the UI thread
                });
            });
        }

        private void AdminPasswordBox_MouseEnter(object sender, MouseEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;

            // Check if the warningPassword label is visible
            if (warningPassword.Visibility == Visibility.Visible)
            {
                SetInvalidInputStyle(passwordBox);
            }
        }

        private void AdminPasswordBox_MouseLeave(object sender, MouseEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;

            // If the password is valid, set the valid input style
            if (ValidatePassword())
            {
                SetValidInputStyle(passwordBox);
            }
            else
            {
                SetInvalidInputStyle(passwordBox);
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            _pageModel.Navigate(new Credits());
        }
    }
}
