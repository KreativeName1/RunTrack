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
        // Definiert den Dialogmodus (Neu oder Bearbeiten)
        private DialogMode mode = DialogMode.Neu;
        // Speichert den Vornamen und Nachnamen des Benutzers
        private string firstName = string.Empty;
        private string lastName = string.Empty;

        // Referenz auf das Hauptmodell
        private MainModel? _mmodel;

        // Konstruktor für die AdminEinstellungen-Seite
        public AdminEinstellungen(DialogMode mode, string firstName = "", string lastName = "")
        {
            InitializeComponent();
            // Initialisiert das Hauptmodell
            _mmodel = FindResource("pmodel") as MainModel ?? new();
            this.mode = mode;

            this.firstName = firstName;
            this.lastName = lastName;

            // Button, um das Passwort zu ändern
            this.btnAendern.Click += (sender, e) =>
            {
                bool result = ChangePassword(this.txtPasswordOld.Password, this.txtPasswordNew.Password);
                if (result) _mmodel?.Navigate(_mmodel.History[^1]);
            };
            // Button, um einen neuen Admin zu erstellen
            this.btnErstellen.Click += (sender, e) => AdminErstellen();

            // Button, um den Dialog zu schließen
            this.btnAbbrechen.Click += (sender, e) => _mmodel?.Navigate(_mmodel.History[^1]);

            // Konfiguriert die UI basierend auf dem Dialogmodus
            if (mode == DialogMode.Neu)
            {
                tbTitel.Text = "Admin anlegen";
                txtPasswordOld.Visibility = Visibility.Collapsed;
                btnAendern.Visibility = Visibility.Collapsed;
                btnErstellen.Visibility = Visibility.Visible;
                paswdWDH.Visibility = Visibility.Collapsed;

                // Wenn Enter gedrückt wird, wird der Button "Admin erstellen" aufgerufen
                this.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.Enter) AdminErstellen();
                };
            }
            else
            {
                tbTitel.Text = "Passwort ändern";
                txtVorname.IsEnabled = false;
                txtNachname.IsEnabled = false;
                txtVorname.Text = firstName;
                txtNachname.Text = lastName;

                btnErstellen.Visibility = Visibility.Collapsed;
                btnAendern.Visibility = Visibility.Visible;

                // Wenn Enter gedrückt wird, wird der Button "Passwort ändern" aufgerufen
                this.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.Enter) ChangePassword(this.txtPasswordOld.Password, this.txtPasswordNew.Password);
                };
            }
        }

        // Wird vom Button "Passwort ändern" aufgerufen
        private bool ChangePassword(string oldPassword, string newPassword)
        {
            if (!ValidateInputsBearbeiten()) return false;

            bool result = false;

            using (var db = new LaufDBContext())
            {
                Benutzer? admin = db.Benutzer.FirstOrDefault(b => b.Vorname == this.firstName && b.Nachname == this.lastName);
                if (admin != null && BCrypt.Net.BCrypt.Verify(oldPassword, admin.Passwort))
                {
                    admin.Passwort = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    db.SaveChanges();
                    new Popup().Display("Passwortänderung", "Das Passwort wurde erfolgreich geändert :)", PopupType.Success, PopupButtons.Ok);
                    result = true;
                }
            }
            return result;
        }

        // Überprüfen, ob das neue Passwort den Sicherheitsanforderungen entspricht
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

        // Überprüfen, ob das alte Passwort korrekt ist
        private bool ValidateOldPassword(string oldPassword)
        {
            using (var db = new LaufDBContext())
            {
                Benutzer? admin = db.Benutzer.FirstOrDefault(b => b.Vorname == this.firstName && b.Nachname == this.lastName);
                if (admin != null && BCrypt.Net.BCrypt.Verify(oldPassword, admin.Passwort)) return true;
                else return false;
            }
        }

        // Überprüfen, ob die Eingaben korrekt sind (für das Anlegen eines neuen Admins)
        private bool ValidateInputsAnlegen()
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(txtVorname.Text)) { warningVorname.Visibility = Visibility.Visible; isValid = false; }
            else warningVorname.Visibility = Visibility.Hidden;
            if (string.IsNullOrWhiteSpace(txtNachname.Text)) { warningNachname.Visibility = Visibility.Visible; isValid = false; }
            else warningNachname.Visibility = Visibility.Hidden;

            // Überprüfen, ob das Passwort den Sicherheitsanforderungen entspricht
            if (string.IsNullOrWhiteSpace(txtPasswordNew.Password) || !ValidateNewPassword(txtPasswordNew.Password))
            {
                warningPasswordNeu.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                warningPasswordNeu.Visibility = Visibility.Hidden;
            }

            // Überprüfen, ob das Passwort mit der Wiederholung übereinstimmt
            if (string.IsNullOrWhiteSpace(txtPasswordNewWdh.Password) || txtPasswordNew.Password != txtPasswordNewWdh.Password)
            {
                warningPasswordWDH.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                warningPasswordWDH.Visibility = Visibility.Hidden;
            }

            return isValid;
        }

        // Überprüfen, ob die Eingaben korrekt sind (für das Bearbeiten eines Admins)
        private bool ValidateInputsBearbeiten()
        {
            bool isValid = true;

            // Überprüfen, ob das Passwort den Sicherheitsanforderungen entspricht
            if (string.IsNullOrWhiteSpace(txtPasswordNew.Password) || !ValidateNewPassword(txtPasswordNew.Password))
            {
                warningPasswordNeu.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                warningPasswordNeu.Visibility = Visibility.Hidden;
            }

            // Überprüfen, ob in der Wiederholung ein Passwort eingegeben wurde
            if (string.IsNullOrWhiteSpace(txtPasswordNewWdh.Password))
            {
                warningPasswordWDH.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                warningPasswordWDH.Visibility = Visibility.Hidden;
            }

            // Überprüfen, ob das Passwort mit der Wiederholung übereinstimmt
            if (txtPasswordNew.Password != txtPasswordNewWdh.Password)
            {
                warningPasswordWDH.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                warningPasswordWDH.Visibility = Visibility.Hidden;
            }

            // Überprüfen, ob das alte Passwort korrekt ist
            if (string.IsNullOrWhiteSpace(txtPasswordOld.Password) || !ValidateOldPassword(txtPasswordOld.Password))
            {
                warningPasswordOld.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                warningPasswordOld.Visibility = Visibility.Hidden;
            }

            return isValid;
        }

        // Wird vom Button "Admin erstellen" aufgerufen oder wenn die Enter-Taste gedrückt wird
        private void AdminErstellen()
        {
            if (!ValidateInputsAnlegen()) return;

            using (var db = new LaufDBContext())
            {
                var existingUser = db.Benutzer.FirstOrDefault(b => b.Vorname == txtVorname.Text && b.Nachname == txtNachname.Text);

                if (existingUser == null)
                {
                    // Wenn das Neue Passwort den Sicherheitsanforderungen entspricht, wird der Benutzer erstellt
                    if (ValidateNewPassword(txtPasswordNew.Password))
                    {
                        Benutzer benutzer = new()
                        {
                            Vorname = txtVorname.Text.ToLower(),
                            Nachname = txtNachname.Text.ToLower(),
                            Passwort = BCrypt.Net.BCrypt.HashPassword(txtPasswordNew.Password),
                        };
                        db.Benutzer.Add(benutzer);
                        db.SaveChanges();
                        new Popup().Display("Erfolg", "Der neue Benutzer wurde erfolgreich erstellt.", PopupType.Success, PopupButtons.Ok);
                    }
                    else
                    {
                        new Popup().Display("Fehler", "Das neue Passwort erfüllt nicht die Sicherheitsanforderungen.", PopupType.Error, PopupButtons.Ok);
                        return;
                    }
                }
                else
                {
                    new Popup().Display("Fehler", "Ein Benutzer mit diesem Namen existiert bereits.", PopupType.Error, PopupButtons.Ok);
                    return;
                }
            }
            _mmodel?.Navigate(_mmodel.History[^1]);
        }

        // Wird aufgerufen, wenn das Fenster geladen wird
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtVorname.ForegroundBrush = new SolidColorBrush(Colors.Gray);
            txtNachname.ForegroundBrush = new SolidColorBrush(Colors.Gray);
        }

        // Wird aufgerufen, wenn der "Credits"-Button geklickt wird
        private void btnCredits_Click(object sender, RoutedEventArgs e)
        {
            _mmodel?.Navigate(new Credits());
        }
    }
}