using FullControls.Controls;
using RunTrack.View;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace RunTrack
{
    public partial class MainWindow : Page
    {
        public ResizeMode ResizeMode { get; set; }
        private MainViewModel _viewModel;
        private PageModel _pageModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = FindResource("mvmodel") as MainViewModel ?? new();
            _pageModel = FindResource("pmodel") as PageModel ?? new();
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
            _viewModel.Benutzer = new Benutzer
            {
                Vorname = FirstNameTextBox.Text,
                Nachname = LastNameTextBox.Text,
            };

            // Überprüfen, ob Vorname und Nachname eingegeben wurden
            if (ValidateInputs())
            {
                using (var db = new LaufDBContext())
                {
                    // Suchen Sie den Benutzer in der Datenbank
                    Benutzer? user = db.Benutzer.FirstOrDefault(b => b.Vorname == _viewModel.Benutzer.Vorname && b.Nachname == _viewModel.Benutzer.Nachname);

                    // Wenn der Benutzer nicht gefunden wurde, melden Sie den Benutzer ohne Passwortüberprüfung an
                    if (user == null)
                    {
                        _viewModel.Benutzer.IsAdmin = false; // Oder true, wenn Sie annehmen möchten, dass der Benutzer Administratorrechte hat
                        _pageModel.Navigate(new Scanner());
                    }
                    else
                    {
                        // Wenn der Benutzer existiert, überprüfen Sie das Passwort
                        _viewModel.Benutzer.Passwort = AdminPasswordBox.Password;
                        if (string.IsNullOrEmpty(AdminPasswordBox.Password))
                        {
                            // Kein Passwort eingegeben, anmelden als normaler Benutzer
                            _viewModel.Benutzer.IsAdmin = false; // Setzen Sie auf true, falls der Benutzer Administratorrechte haben soll
                            _pageModel.Navigate(new Scanner());
                        }
                        else if (BCrypt.Net.BCrypt.Verify(AdminPasswordBox.Password, user.Passwort))
                        {
                            // Passwort stimmt überein, anmelden als Administrator
                            _viewModel.Benutzer.IsAdmin = true;

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
                        warningPassword.Visibility = Visibility.Collapsed; // Kein Passwort nötig, wenn der Benutzer existiert
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
                        warningPassword.Visibility = Visibility.Collapsed; // Warnung ausblenden, wenn das Passwort korrekt ist
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
                warningVorname.Visibility = Visibility.Collapsed;
            }
            else if (textBox.Name == "LastNameTextBox" && warningNachname != null)
            {
                warningNachname.Visibility = Visibility.Collapsed;
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
                        btnLogin.Margin = new Thickness(btnLogin.Margin.Left, 260, btnLogin.Margin.Right, btnLogin.Margin.Bottom);
                    }
                    else
                    {
                        // Benutzer nicht gefunden, Passwortfeld ausblenden
                        gridPasswordLable.Visibility = Visibility.Collapsed;
                        borderPassword.Visibility = Visibility.Collapsed;
                        warningPassword.Visibility = Visibility.Collapsed;
                        btnLogin.Margin = new Thickness(btnLogin.Margin.Left, 200, btnLogin.Margin.Right, btnLogin.Margin.Bottom);
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
                        btnLogin.Margin = new Thickness(btnLogin.Margin.Left, 260, btnLogin.Margin.Right, btnLogin.Margin.Bottom);
                    }
                    else
                    {
                        gridPasswordLable.Visibility = Visibility.Collapsed;
                        borderPassword.Visibility = Visibility.Collapsed;
                        warningPassword.Visibility = Visibility.Collapsed;
                        btnLogin.Margin = new Thickness(btnLogin.Margin.Left, 200, btnLogin.Margin.Right, btnLogin.Margin.Bottom);
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
                    borderPassword.Visibility = Visibility.Collapsed;
                }
            }

            ValidatePassword();
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
