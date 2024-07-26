using FullControls.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Klimalauf.View
{
    /// <summary>
    /// Interaktionslogik für AdminEinstellungen.xaml
    /// </summary>
    public partial class AdminEinstellungen : Window
    {
        private DialogMode mode = DialogMode.Neu;

        private string firstName = string.Empty;
        private string lastName = string.Empty;

        public AdminEinstellungen()
        {
            InitializeComponent();
        }

        public AdminEinstellungen(DialogMode mode, string firstName = "", string lastName = "")
        {
            InitializeComponent();
            this.mode = mode;
            this.firstName = firstName;
            this.lastName = lastName;

            SetWindowSize();

            this.txtVorname.Text = firstName;
            this.txtNachname.Text = lastName;

            if (mode == DialogMode.Bearbeiten)
            {
                this.txtPasswordOld.Focus();
                this.btnAendern.Visibility = Visibility.Visible;
                this.btnErstellen.Visibility = Visibility.Collapsed;
                this.paswdWDH.Visibility = Visibility.Visible;
                this.txtVorname.IsEnabled = false;
                this.txtNachname.IsEnabled = false;
                this.txtPasswordNew.IsEnabled = false;

                this.btnAendern.Click += (sender, e) =>
                {
                    ChangePassword(this.txtPasswordOld.Password, this.txtPasswordNew.Password);
                };
            }
            else if (mode == DialogMode.Neu)
            {
                this.txtVorname.Focus();
                this.txtVorname.IsEnabled = true;
                this.txtNachname.IsEnabled = true;
                this.btnAendern.Visibility = Visibility.Collapsed;
                this.btnErstellen.Visibility = Visibility.Visible;
                this.txtPasswordNew.IsEnabled = true;
                this.paswdWDH.Visibility = Visibility.Collapsed;
            }
        }

        private void SetWindowSize()
        {
            if (mode == DialogMode.Bearbeiten)
            {
                this.Width = 460;
                this.Height = 440.5;
            }
            else if (mode == DialogMode.Neu)
            {
                this.warningPassword.Margin = new Thickness(368, 192, 0, 0);
                this.newPasswd.Margin = new Thickness(87, 176, 0, 0);
                this.borderPasswordWdh.Margin = new Thickness(87, 216, 0, 0);
                this.btnErstellen.Margin = new Thickness(0, 280, 0, 0);
                this.Width = 460;
                this.Height = 400.5;
            }
        }

        private bool ChangePassword(string oldPassword, string newPassword)
        {
            bool result = false;
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
            // Beispielhafte Validierungsanforderungen für ein sicheres Passwort
            return !string.IsNullOrEmpty(newPassword) &&
                   newPassword.Length >= 1;
            //&&
            //newPassword.Any(char.IsUpper) &&
            //newPassword.Any(char.IsLower) &&
            //newPassword.Any(char.IsDigit);
        }

        private void btnErstellen_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                using (var db = new LaufDBContext())
                {
                    if (mode == DialogMode.Neu)
                    {
                        var existingUser = db.Benutzer.FirstOrDefault(b => b.Vorname == txtVorname.Text && b.Nachname == txtNachname.Text);

                        if (existingUser == null)
                        {
                            if (ValidateNewPassword(txtPasswordNew.Password))
                            {
                                // Überprüfen, ob das Passwort bereits von einem anderen Benutzer verwendet wird
                                bool passwordInUse = db.Benutzer.Any(b => b.Passwort == BCrypt.Net.BCrypt.HashPassword(txtPasswordNew.Password));
                                if (passwordInUse)
                                {
                                    new Popup().Display("Fehler", "Das Passwort wird bereits von einem anderen Benutzer verwendet.", PopupType.Error, PopupButtons.Ok);
                                }
                                else
                                {
                                    Benutzer benutzer = new()
                                    {
                                        Vorname = txtVorname.Text,
                                        Nachname = txtNachname.Text,
                                        Passwort = BCrypt.Net.BCrypt.HashPassword(txtPasswordNew.Password)
                                    };
                                    db.Benutzer.Add(benutzer);
                                    db.SaveChanges();
                                    new Popup().Display("Erfolg", "Benutzer erfolgreich erstellt :)", PopupType.Success, PopupButtons.Ok);
                                }
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
                    else if (mode == DialogMode.Bearbeiten)
                    {
                        // Find the existing user and update the password
                        var existingUser = db.Benutzer.FirstOrDefault(b => b.Vorname == this.firstName && b.Nachname == this.lastName);

                        if (existingUser != null)
                        {
                            if (ValidateNewPassword(txtPasswordNew.Password))
                            {
                                bool passwordInUse = db.Benutzer.Any(b => b.Passwort == BCrypt.Net.BCrypt.HashPassword(txtPasswordNew.Password) && !(b.Vorname == this.firstName && b.Nachname == this.lastName));
                                if (passwordInUse)
                                {
                                    new Popup().Display("Fehler", "Das Passwort wird bereits von einem anderen Benutzer verwendet.", PopupType.Error, PopupButtons.Ok);
                                }
                                else
                                {
                                    existingUser.Passwort = BCrypt.Net.BCrypt.HashPassword(txtPasswordNew.Password);
                                    db.SaveChanges();
                                    new Popup().Display("Erfolg", "Passwort erfolgreich geändert :)", PopupType.Success, PopupButtons.Ok);
                                }
                            }
                            else
                            {
                                new Popup().Display("Fehler", "Das neue Passwort erfüllt nicht die Sicherheitsanforderungen.", PopupType.Error, PopupButtons.Ok);
                            }
                        }
                        else
                        {
                            new Popup().Display("Fehler", "Der Benutzer existiert nicht.", PopupType.Error, PopupButtons.Ok);
                        }
                    }
                }
                this.Close();
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
                SetInvalidInputStyle(txtPasswordNew);
                txtPasswordNewWdh.Password = "";
                isValid = false;
            }
            else
            {
                SetValidInputStyle(txtPasswordNew);
            }

            // Überprüfe, ob das Passwort zur Bestätigung ausgefüllt ist
            if (string.IsNullOrEmpty(txtPasswordNewWdh.Password) || txtPasswordNew.Password != txtPasswordNewWdh.Password)
            {
                SetInvalidInputStyle(txtPasswordNewWdh);
                SetInvalidInputStyle(txtPasswordNew); // Setze rote Unterstreichung für beide
                warningPassword.Visibility = Visibility.Visible; // Warnung anzeigen
                isValid = false;
            }
            else
            {
                SetValidInputStyle(txtPasswordNewWdh);
                SetValidInputStyle(txtPasswordNew);
                warningPassword.Visibility = Visibility.Collapsed; // Warnung ausblenden
            }

            // Überprüfe, ob sich das neue Passwort vom alten unterscheidet (inkl. Groß- und Kleinschreibung)
            if (!string.IsNullOrEmpty(txtPasswordOld.Password) && string.Equals(txtPasswordNew.Password, txtPasswordOld.Password, StringComparison.Ordinal))
            {
                SetInvalidInputStyle(txtPasswordNew);
                warningPassword.Visibility = Visibility.Visible;
                isValid = false;
            }

            return isValid;
        }


        private void SetInvalidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush(Colors.Red);
            passwordBox.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
            if (passwordBox.Name == "txtPasswordOld")
                warningPasswordOld.Visibility = Visibility.Visible;
            else if (passwordBox.Name == "txtPasswordNew" || passwordBox.Name == "txtPasswordNewWdh")
                warningPassword.Visibility = Visibility.Visible;
        }


        private void SetValidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
            warningPassword.Visibility = Visibility.Collapsed;
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
                    btnErstellen_Click(sender, e);
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

            // Sichtbarkeit der Passwortwiederholung und Anpassung der Button-Position
            if (!string.IsNullOrEmpty(passwordBox.Password))
            {
                borderPasswordWdh.Visibility = Visibility.Visible;
                btnErstellen.Margin = new Thickness(0, 260, 0, 0); // Button nach unten verschieben
            }
            else
            {
                borderPasswordWdh.Visibility = Visibility.Collapsed;
                btnErstellen.Margin = new Thickness(0, 238, 0, 0); // Button nach oben verschieben
            }

            ValidatePassword(); // Passwortvalidierung durchführen
        }

        private void btnCredits_Click(object sender, RoutedEventArgs e)
        {
            Credits cr = new();
            cr.ShowDialog();
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
            if (!string.IsNullOrEmpty(txtPasswordOld.Password))
            {
                ValidateOldPassword(txtPasswordOld.Password);
            }
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
                    warningPassword.Visibility = Visibility.Collapsed;
                }
            }

            PasswordBoxPlus passwordBox = (PasswordBoxPlus)sender;

            // Dynamische Überprüfung des alten Passworts
            ValidateOldPassword(passwordBox.Password);
        }

    }
}
