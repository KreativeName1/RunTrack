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
                this.btnAendern.Visibility = Visibility.Visible;
                this.btnErstellen.Visibility = Visibility.Collapsed;
                this.paswdWDH.Visibility = Visibility.Visible;
                this.txtVorname.IsEnabled = false;
                this.txtNachname.IsEnabled = false;

                this.btnAendern.Click += (sender, e) =>
                {
                    ChangePassword(this.txtPasswordOld.Password, this.txtPasswordNew.Password);
                };
            }
            else if (mode == DialogMode.Neu)
            {
                this.txtVorname.IsEnabled = true;
                this.txtNachname.IsEnabled = true;
                this.btnAendern.Visibility = Visibility.Collapsed;
                this.btnErstellen.Visibility = Visibility.Visible;
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
                newPasswd.Margin = new Thickness(87, 176, 0, 0);
                borderPasswordWdh.Margin = new Thickness(87, 216, 0, 0);
                btnErstellen.Margin = new Thickness(0, 280, 0, 0);
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
                    admin.Passwort = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    db.SaveChanges();
                    MessageBox.Show("Das Passwort wurde erfolgreich geändert :)", "Passwortänderung erfolgreich", MessageBoxButton.OK);
                    result = true;
                }
                else
                {
                    MessageBox.Show("Das alte Passwort ist falsch.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return result;
        }


        private void btnErstellen_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                using (var db = new LaufDBContext())
                {
                    // Überprüfen, ob der Benutzer bereits existiert
                    var existingUser = db.Benutzer.FirstOrDefault(b => b.Vorname == txtVorname.Text && b.Nachname == txtNachname.Text);

                    if (existingUser == null)
                    {
                        // Benutzer anlegen mit gehashtem Passwort
                        Benutzer benutzer = new Benutzer
                        {
                            Vorname = txtVorname.Text,
                            Nachname = txtNachname.Text,
                            Passwort = BCrypt.Net.BCrypt.HashPassword(txtPasswordNew.Password)
                        };
                        db.Benutzer.Add(benutzer);
                        db.SaveChanges();
                        MessageBox.Show("Benutzer erfolgreich erstellt :)", "Erfolg", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Ein Benutzer mit diesem Namen existiert bereits.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                this.Close();
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtVorname.Focus();
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

            if (string.IsNullOrEmpty(txtPasswordNewWdh.Password) || txtPasswordNew.Password != txtPasswordNewWdh.Password)
            {
                SetInvalidInputStyle(txtPasswordNewWdh);
                isValid = false;
                warningPassword.Visibility = Visibility.Visible; // Warning anzeigen, wenn die Passwörter nicht übereinstimmen
            }
            else
            {
                SetValidInputStyle(txtPasswordNewWdh);
                warningPassword.Visibility = Visibility.Collapsed; // Warning ausblenden, wenn die Passwörter übereinstimmen
            }

            return isValid;
        }

        private void SetInvalidInputStyle(PasswordBoxPlus passwordBox)
        {
            passwordBox.UnderlineBrush = new SolidColorBrush(Colors.Red);
            passwordBox.UnderlineThickness = new Thickness(0, 0, 0, 2.5);
            if (passwordBox.Name == "txtPasswort" || passwordBox.Name == "txtPasswortWdh")
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

            if (!string.IsNullOrEmpty(passwordBox.Password))
            {
                borderPasswordWdh.Visibility = Visibility.Visible;
                btnErstellen.Margin = new Thickness(0, 260, 0, 0); // Move the button down
            }
            else
            {
                borderPasswordWdh.Visibility = Visibility.Collapsed;

                btnErstellen.Margin = new Thickness(0, 238, 0, 0); // Move the button up
            }

            ValidatePassword();
        }

        private void btnCredits_Click(object sender, RoutedEventArgs e)
        {
            Credits cr = new Credits();
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

    }
}
