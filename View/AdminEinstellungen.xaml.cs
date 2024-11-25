using FullControls.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für AdminEinstellungen.xaml
    /// </summary>
    public partial class AdminEinstellungen : Page
    {
        private bool isPasswordRepeatVisible = false;

        public static ResizeMode ResizeMode { get; set; } = ResizeMode.NoResize;
        private DialogMode mode = DialogMode.Neu;

        private string firstName = string.Empty;
        private string lastName = string.Empty;

        private MainModel? _mmodel;

        public AdminEinstellungen(DialogMode mode, string firstName = "", string lastName = "")
        {
            InitializeComponent();
            _mmodel = FindResource("pmodel") as MainModel ?? new();
            this.mode = mode;
            this.firstName = firstName;
            this.lastName = lastName;
            if (mode == DialogMode.Neu) tbTitel.Text = "Admin anlegen";
            else tbTitel.Text = "Passwort ändern";

            SetWindowSize();

            this.txtVorname.Text = firstName;
            this.txtNachname.Text = lastName;

            txtVorname.IsEnabled = mode == DialogMode.Neu;
            txtNachname.IsEnabled = mode == DialogMode.Neu;
            btnAendern.Visibility = mode == DialogMode.Bearbeiten ? Visibility.Visible : Visibility.Collapsed;
            btnErstellen.Visibility = mode == DialogMode.Neu ? Visibility.Visible : Visibility.Collapsed;

            this.btnAendern.Click += (sender, e) =>
            {
                bool result = ChangePassword(this.txtPasswordOld.Password, this.txtPasswordNew.Password);
                if (result) _mmodel?.Navigate(_mmodel.History[^1]);
            };
            this.btnErstellen.Click += (sender, e) => AdminErstellen();
            this.btnAbbrechen.Click += (sender, e) => _mmodel?.Navigate(_mmodel.History[^1]);

            if (mode == DialogMode.Bearbeiten)
            {
                this.paswdWDH.Visibility = Visibility.Visible;
                this.txtPasswordNew.IsEnabled = false;
                this.txtPasswordNewWdh.IsEnabled = false;
                this.Title = "Passwort bearbeiten";
            }
            else if (mode == DialogMode.Neu)
            {
                this.txtVorname.Focus();
                this.txtPasswordNew.IsEnabled = true;
                this.paswdWDH.Visibility = Visibility.Collapsed;
                this.Title = "Admin anlegen";
            }
        }

        private void SetWindowSize()
        {
            if (mode != DialogMode.Neu) return;
            //this.warningPasswordWDH.Margin = new Thickness(0, 0, 0, 0);
            //this.newPasswd.Margin = new Thickness(87, 176, 0, 0);
            //this.borderPasswordWdh.Margin = new Thickness(0, 0, 0, 0);
        }

        private bool ChangePassword(string oldPassword, string newPassword)
        {
            bool result = false;

            // Überprüfen, ob das neue Passwort mit der Wiederholung übereinstimmt
            if (newPassword != txtPasswordNewWdh.Password)
            {
                new Popup().Display("Fehler", "Die Passwörter stimmen nicht überein.", PopupType.Error, PopupButtons.Ok);
                return false;
            }

            using (var db = new LaufDBContext())
            {
                Benutzer? admin = db.Benutzer.FirstOrDefault(b => b.Vorname == this.firstName && b.Nachname == this.lastName);
                if (admin != null && BCrypt.Net.BCrypt.Verify(oldPassword, admin.Passwort))
                {
                    if (ValidateNewPassword(newPassword))
                    {
                        // Überprüfe, ob sich das neue Passwort vom alten Passwort unterscheidet (inkl. Groß- und Kleinschreibung)
                        if (string.Equals(newPassword, oldPassword, StringComparison.Ordinal))
                        {
                            new Popup().Display("Fehler", "Das neue Passwort darf nicht mit dem alten Passwort übereinstimmen.", PopupType.Error, PopupButtons.Ok);
                        }
                        else
                        {
                            // Überprüfen, ob das Passwort bereits von einem anderen Benutzer verwendet wird
                            bool passwordInUse = db.Benutzer.Any(b => b.Passwort == BCrypt.Net.BCrypt.HashPassword(newPassword) && !(b.Vorname == this.firstName && b.Nachname == this.lastName));
                            if (passwordInUse)
                            {
                                new Popup().Display("Fehler", "Das neue Passwort wird bereits von einem anderen Benutzer verwendet.", PopupType.Error, PopupButtons.Ok);
                            }
                            else
                            {
                                admin.Passwort = BCrypt.Net.BCrypt.HashPassword(newPassword);
                                db.SaveChanges();
                                new Popup().Display("Passwortänderung", "Das Passwort wurde erfolgreich geändert :)", PopupType.Success, PopupButtons.Ok);
                                result = true;
                            }
                        }
                    }
                    else
                    {
                        new Popup().Display("Fehler", "Das neue Passwort erfüllt nicht die Sicherheitsanforderungen.", PopupType.Error, PopupButtons.Ok);
                    }
                }
                else
                {
                    new Popup().Display("Fehler", "Das alte Passwort ist falsch.", PopupType.Error, PopupButtons.Ok);
                }
            }
            return result;
        }


        private bool ValidateNewPassword(string newPassword)
        {
#if DEBUG
            return !string.IsNullOrEmpty(newPassword) && newPassword.Length >= 1;
#endif
            return !string.IsNullOrEmpty(newPassword) &&
                   newPassword.Length >= 8 &&
            newPassword.Any(char.IsUpper) &&
            newPassword.Any(char.IsLower) &&
            newPassword.Any(char.IsDigit) && newPassword.Any(char.IsSymbol);
        }

        private void AdminErstellen()
        {
            if (ValidateInputs() && ValidatePassword()) // Stelle sicher, dass das Passwort validiert wird
            {
                using (var db = new LaufDBContext())
                {
                    var existingUser = db.Benutzer.FirstOrDefault(b => b.Vorname == txtVorname.Text && b.Nachname == txtNachname.Text);

                    if (existingUser == null)
                    {
                        if (ValidateNewPassword(txtPasswordNew.Password))
                        {
                            Benutzer benutzer = new()
                            {
                                Vorname = txtVorname.Text.ToLower(),
                                Nachname = txtNachname.Text.ToLower(),
                                Passwort = BCrypt.Net.BCrypt.HashPassword(txtPasswordNew.Password)
                            };
                            db.Benutzer.Add(benutzer);
                            db.SaveChanges();
                            new Popup().Display("Erfolg", "Der neue Benutzer wurde erfolgreich erstellt.", PopupType.Success, PopupButtons.Ok);

                        }
                        else
                        {
                            new Popup().Display("Fehler", "Das neue Passwort erfüllt nicht die Sicherheitsanforderungen.", PopupType.Error, PopupButtons.Ok);
                        }
                    }
                    else
                    {
                        new Popup().Display("Fehler", "Ein Benutzer mit diesem Namen existiert bereits.", PopupType.Error, PopupButtons.Ok);
                    }
                }
                _mmodel?.Navigate(_mmodel.History[^1]);
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtVorname.ForegroundBrush = new SolidColorBrush(Colors.Gray);
            txtNachname.ForegroundBrush = new SolidColorBrush(Colors.Gray);
        }

        private bool ValidateInputs()
        {
            bool isValid = true;

            if (!ValidateVorname())
            {
                isValid = false;
            }

            if (!ValidateNachname())
            {
                isValid = false;
            }

            if (!ValidatePassword())
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateVorname()
        {
            if (string.IsNullOrWhiteSpace(txtVorname.Text) || txtVorname.Text == "Max")
            {
                SetInvalidInputStyle(txtVorname);
                return false;
            }
            else
            {
                SetValidInputStyle(txtVorname);
                return true;
            }
        }

        private bool ValidateNachname()
        {
            if (string.IsNullOrWhiteSpace(txtNachname.Text) || txtNachname.Text == "Mustermann")
            {
                SetInvalidInputStyle(txtNachname);
                return false;
            }
            else
            {
                SetValidInputStyle(txtNachname);
                return true;
            }
        }

        private bool ValidatePassword()
        {
            bool isValid = true;

            // Überprüfe, ob das neue Passwort gültig ist
            if (string.IsNullOrEmpty(txtPasswordNew.Password))
            {
                txtPasswordNew.UnderlineBrush = new SolidColorBrush(Colors.Red);
                txtPasswordNew.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
                txtPasswordNewWdh.Password = "";
                txtPasswordNewWdh.IsEnabled = false;
                isValid = false;
            }
            else
            {
                txtPasswordNew.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
                txtPasswordNew.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
            }

            // Überprüfe, ob das Passwort zur Bestätigung ausgefüllt ist
            if (string.IsNullOrEmpty(txtPasswordNewWdh.Password))
            {
                txtPasswordNewWdh.UnderlineBrush = new SolidColorBrush(Colors.Red);
                txtPasswordNewWdh.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
                warningPasswordWDH.Visibility = Visibility.Collapsed;
                isValid = false;
            }
            else if (txtPasswordNew.Password != txtPasswordNewWdh.Password)
            {
                // Wenn die Passwörter nicht übereinstimmen
                txtPasswordNewWdh.UnderlineBrush = new SolidColorBrush(Colors.Red);
                txtPasswordNewWdh.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
                warningPasswordWDH.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                txtPasswordNewWdh.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
                txtPasswordNewWdh.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
                warningPasswordWDH.Visibility = Visibility.Collapsed; // Ausblenden, wenn die Passwörter übereinstimmen
            }

            // Überprüfe, ob sich das neue Passwort vom alten unterscheidet
            if (!string.IsNullOrEmpty(txtPasswordOld.Password) && string.Equals(txtPasswordNew.Password, txtPasswordOld.Password, StringComparison.Ordinal))
            {
                txtPasswordNew.UnderlineBrush = new SolidColorBrush(Colors.Red);
                txtPasswordNew.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
                warningPasswordNeuAlt.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!string.IsNullOrEmpty(txtPasswordOld.Password) && !string.Equals(txtPasswordNew.Password, txtPasswordOld.Password, StringComparison.Ordinal))
            {
                warningPasswordNeuAlt.Visibility = Visibility.Collapsed;

                if (ValidateNewPassword(txtPasswordNew.Password))
                {
                    txtPasswordNew.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
                    txtPasswordNew.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
                    txtPasswordNewWdh.IsEnabled = true;
                    warningPasswordNeu.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtPasswordNew.UnderlineBrush = new SolidColorBrush(Colors.Red);
                    txtPasswordNew.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
                    txtPasswordNewWdh.Password = "";
                    txtPasswordNewWdh.IsEnabled = false;
                    warningPasswordNeu.Visibility = Visibility.Visible;
                }
            }

            return isValid;
        }

        private void SetInvalidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush(Colors.Red);
            passwordBox.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
            if (passwordBox.Name == "txtPasswordOld")
                warningPasswordOld.Visibility = Visibility.Visible;
        }


        private void SetValidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
            //warningPasswordWDH.Visibility = Visibility.Collapsed;
            passwordBox.UnderlineThickness = new Thickness(0, 0, 0, 2);
        }

        private void SetInvalidInputStyle(TextBoxPlus textBox)
        {
            textBox.UnderlineBrush = new SolidColorBrush(Colors.Red);
            textBox.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
            if (textBox.Name == "txtVorname")
                warningVorname.Visibility = Visibility.Visible;
            else if (textBox.Name == "txtNachname")
                warningNachname.Visibility = Visibility.Visible;
        }

        private void SetValidInputStyle(TextBoxPlus textBox)
        {
            textBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
            textBox.UnderlineThickness = new Thickness(0, 0, 0, 2);

            if (textBox.Name == "txtVorname")
                warningVorname.Visibility = Visibility.Collapsed;
            else if (textBox.Name == "txtNachname")
                warningNachname.Visibility = Visibility.Collapsed;
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
                    AdminErstellen();
                }
            }
        }

        private void txtVorname_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            textBox.ForegroundBrush = new SolidColorBrush(Colors.Blue);
        }

        private void txtNachname_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            textBox.ForegroundBrush = new SolidColorBrush(Colors.Blue);
        }

        private void txtVorname_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (textBox.Text == "Max")
            {
                textBox.Text = "";
            }
            SetValidInputStyle(textBox);
        }

        private void txtVorname_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Max";
                textBox.ForegroundBrush = new SolidColorBrush(Colors.Gray);
            }
            ValidateVorname();
        }

        private void txtNachname_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (textBox.Text == "Mustermann")
            {
                textBox.Text = "";
            }
            SetValidInputStyle(textBox);
        }

        private void txtNachname_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPlus textBox = (TextBoxPlus)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Mustermann";
                textBox.ForegroundBrush = new SolidColorBrush(Colors.Gray);
            }
            ValidateNachname();
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

            // Sobald die Wiederholung des Passworts eingeblendet wurde, soll sie nicht mehr ausgeblendet werden
            if (!string.IsNullOrEmpty(passwordBox.Password) || isPasswordRepeatVisible)
            {
                borderPasswordWdh.Visibility = Visibility.Visible;
                isPasswordRepeatVisible = true; // Behalte den Zustand
            }

            ValidatePassword(); // Passwortvalidierung durchführen
        }


        private void btnCredits_Click(object sender, RoutedEventArgs e)
        {
            _mmodel?.Navigate(new Credits());
        }

        private void txtPasswortWdh_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
        }

        private void txtPasswortWdh_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;
            passwordBox.Foreground = new SolidColorBrush(Colors.Blue);
            ValidatePassword();
        }


        private void txtPasswordOld_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPasswordOld.Password)) ValidateOldPassword(txtPasswordOld.Password);
        }

        private void ValidateOldPassword(string oldPassword)
        {
            using (var db = new LaufDBContext())
            {
                Benutzer? admin = db.Benutzer.FirstOrDefault(b => b.Vorname == this.firstName && b.Nachname == this.lastName);
                if (admin != null && BCrypt.Net.BCrypt.Verify(oldPassword, admin.Passwort))
                {
                    txtPasswordNew.IsEnabled = true;
                    SetValidInputStyle(txtPasswordOld);
                    warningPasswordOld.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtPasswordNew.IsEnabled = false;
                    SetInvalidInputStyle(txtPasswordOld);
                    warningPasswordOld.Visibility = Visibility.Visible;
                }
            }
        }

        private void txtPasswordOld_PasswordChanged(object sender, RoutedEventArgs e)
        {
            using (var db = new LaufDBContext())
            {
                Benutzer? benutzer = db.Benutzer.FirstOrDefault(b => b.Vorname == txtVorname.Text && b.Nachname == txtNachname.Text);
                if (benutzer != null && BCrypt.Net.BCrypt.Verify(txtPasswordOld.Password, benutzer.Passwort))
                {
                    txtPasswordNew.IsEnabled = true;
                }
                else
                {
                    txtPasswordNew.Password = "";
                    txtPasswordNew.IsEnabled = false;
                    warningPasswordWDH.Visibility = Visibility.Collapsed;
                }
            }

            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;

            // Dynamische Überprüfung des alten Passworts
            ValidateOldPassword(passwordBox.Password);
        }
    }
}