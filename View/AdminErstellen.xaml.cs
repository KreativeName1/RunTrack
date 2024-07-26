using FullControls.Controls;
using RunTrack.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für AdminErstellen.xaml
    /// </summary>
    public partial class AdminErstellen : Window
    {
        public AdminErstellen()
        {
            InitializeComponent();
        }

        private void btnErstellen_Click(object sender, RoutedEventArgs e)
        {
            // prüfen, ob Benutzername und Passwort eingegeben wurden
            if (ValidateInputs())
            {
                using (var db = new LaufDBContext())
                {
                    // Benutzer anlegen mit gehashtem Passwort
                    Benutzer benutzer = new();
                    benutzer.Vorname = txtVorname.Text;
                    benutzer.Nachname = txtNachname.Text;
                    benutzer.Passwort = BCrypt.Net.BCrypt.HashPassword(txtPassword.Password);
                    db.Benutzer.Add(benutzer);
                    db.SaveChanges();
                }
                MainWindow mainWindow = new();
                mainWindow.Show();
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

            // Passwortfelder leeren, wenn das Passwort leer ist
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                SetInvalidInputStyle(txtPassword);
                txtPasswordWdh.Password = "";
                isValid = false;
            }
            else
            {
                SetValidInputStyle(txtPassword);
            }

            // Überprüfen, ob das wiederholte Passwort leer ist oder nicht mit dem Passwort übereinstimmt
            if (string.IsNullOrEmpty(txtPasswordWdh.Password) || txtPassword.Password != txtPasswordWdh.Password)
            {
                SetInvalidInputStyle(txtPasswordWdh);
                isValid = false;
                warningPassword.Visibility = Visibility.Visible; // Warning anzeigen, wenn die Passwörter nicht übereinstimmen
            }
            else
            {
                SetValidInputStyle(txtPasswordWdh);
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
    }
}
