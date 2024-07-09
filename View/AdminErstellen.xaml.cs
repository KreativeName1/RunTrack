using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BCrypt.Net;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für AdminErstellen.xaml
    /// </summary>
    public partial class AdminErstellen : Window
    {
        public AdminErstellen()
        {
            InitializeComponent();

            btnErstellen.Click += (s, e) =>
            {
                // prüfen, ob Benutzername und Passwort eingegeben wurden
                if (string.IsNullOrEmpty(txtVorname.Text) || string.IsNullOrEmpty(txtNachname.Text) || string.IsNullOrEmpty(txtPasswort.Password))
                {
                    MessageBox.Show("Bitte Benutzername und Passwort eingeben");
                    return;
                }
                using (var db = new LaufDBContext())
                {
                    // Benutzer anlegen mit gehashtem Passwort
                    Benutzer benutzer = new Benutzer();
                    benutzer.Vorname = txtVorname.Text;
                    benutzer.Nachname = txtNachname.Text;
                    benutzer.Passwort = BCrypt.Net.BCrypt.HashPassword(txtPasswort.Password);
                    db.Benutzer.Add(benutzer);
                    db.SaveChanges();
                }
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            };
        }
    }
}
