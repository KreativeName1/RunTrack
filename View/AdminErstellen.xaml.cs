using FullControls.Controls;
using Klimalauf.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
      }

      private void btnErstellen_Click(object sender, RoutedEventArgs e)
      {
         // prüfen, ob Benutzername und Passwort eingegeben wurden
         if (ValidateInputs())
         {
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
         }
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         txtVorname.Focus();
         txtVorname.Foreground = new SolidColorBrush(Colors.Gray);
         txtNachname.Foreground = new SolidColorBrush(Colors.Gray);
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

         if (!ValidatePasswort())
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

      private bool ValidatePasswort()
      {
         bool isValid = true;

         // Passwortfelder leeren, wenn das Passwort leer ist
         if (string.IsNullOrEmpty(txtPasswort.Password))
         {
            SetInvalidInputStyle(txtPasswort);
            txtPasswortWdh.Password = "";
            isValid = false;
         }
         else
         {
            SetValidInputStyle(txtPasswort);
         }

         // Überprüfen, ob das wiederholte Passwort leer ist oder nicht mit dem Passwort übereinstimmt
         if (string.IsNullOrEmpty(txtPasswortWdh.Password) || txtPasswort.Password != txtPasswortWdh.Password)
         {
            SetInvalidInputStyle(txtPasswortWdh);
            isValid = false;
            warningPassword.Visibility = Visibility.Visible; // Warning anzeigen, wenn die Passwörter nicht übereinstimmen
         }
         else
         {
            SetValidInputStyle(txtPasswortWdh);
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

      private void SetInvalidInputStyle(PasswordBox passwordBox)
      {
         passwordBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C1121C"));
         passwordBox.Foreground = new SolidColorBrush(Colors.White);
      }

      private void SetValidInputStyle(PasswordBox passwordBox)
      {
         passwordBox.Background = new SolidColorBrush(Colors.White);
         passwordBox.Foreground = new SolidColorBrush(Colors.Blue);
      }

      private void TextBox_KeyDown(object sender, KeyEventArgs e)
      {
         //if (sender is PasswordBox pwBox)
         //{
         //   this.lblPasswortWdh.Visibility = Visibility.Visible;
         //   this.txtPasswortWdh.Visibility = Visibility.Visible;

         //   txtPasswortWdh.Background = new SolidColorBrush(Colors.White);
         //   txtPasswortWdh.Foreground = new SolidColorBrush(Colors.Blue);

         //   // lstlastScan.Margin = new Thickness(lstlastScan.Margin.Left, lstlastScan.Margin.Top, lstlastScan.Margin.Right, 100);

         //   this.btnErstellen.Margin = new Thickness(btnErstellen.Margin.Left, btnErstellen.Margin.Top, btnErstellen.Margin.Right, 30);
         //}
         //else
         //{
         //   this.btnErstellen.Margin = new Thickness(btnErstellen.Margin.Left, btnErstellen.Margin.Top, btnErstellen.Margin.Right, 40);

         //   if (txtPasswortWdh.Password.ToString() == "")
         //   {
         //      this.lblPasswortWdh.Visibility = Visibility.Collapsed;
         //      this.txtPasswortWdh.Visibility = Visibility.Collapsed;
         //   }
         //}

         if (e.Key == Key.Enter)
         {
            if (sender is TextBox textBox)
            {
               textBox.Background = new SolidColorBrush(Colors.White);
               textBox.Foreground = new SolidColorBrush(Colors.Blue);
            }
            else if (sender is PasswordBox passwordBox)
            {
               passwordBox.Background = new SolidColorBrush(Colors.White);
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
         TextBox textBox = (TextBox)sender;
         textBox.Background = new SolidColorBrush(Colors.White);
         textBox.Foreground = new SolidColorBrush(Colors.Blue);
      }

      private void txtNachname_TextChanged(object sender, TextChangedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         textBox.Background = new SolidColorBrush(Colors.White);
         textBox.Foreground = new SolidColorBrush(Colors.Blue);
      }

      private void txtVorname_GotFocus(object sender, RoutedEventArgs e)
      {
         TextBoxPlus textBox = (TextBoxPlus)sender;
         if (textBox.Text == "Max")
         {
            textBox.Text = "";
            textBox.Foreground = new SolidColorBrush(Colors.Blue);
         }
         SetValidInputStyle(textBox);
      }

      private void txtVorname_LostFocus(object sender, RoutedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         if (string.IsNullOrWhiteSpace(textBox.Text))
         {
            textBox.Text = "Max";
            textBox.Foreground = new SolidColorBrush(Colors.Gray);
         }
         ValidateVorname();
      }

      private void txtNachname_GotFocus(object sender, RoutedEventArgs e)
      {
         TextBoxPlus textBox = (TextBoxPlus)sender;
         if (textBox.Text == "Mustermann")
         {
            textBox.Text = "";
            textBox.Foreground = new SolidColorBrush(Colors.Blue);
         }
         SetValidInputStyle(textBox);
      }

      private void txtNachname_LostFocus(object sender, RoutedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         if (string.IsNullOrWhiteSpace(textBox.Text))
         {
            textBox.Text = "Mustermann";
            textBox.Foreground = new SolidColorBrush(Colors.Gray);
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
         if (!string.IsNullOrEmpty(passwordBox.Password))
         {
            gridPasswortWdh.Visibility = Visibility.Visible;
            btnErstellen.Margin = new Thickness(0, 260, 0, 0); // Move the button down
         }
         else
         {
            gridPasswortWdh.Visibility = Visibility.Collapsed;
            btnErstellen.Margin = new Thickness(0, 238, 0, 0); // Move the button up
         }

         ValidatePasswort();
      }

      private void btnCredits_Click(object sender, RoutedEventArgs e)
      {
         Credits cr = new Credits();
         cr.ShowDialog();
      }

      private void txtPasswortWdh_LostFocus(object sender, RoutedEventArgs e)
      {
         ValidatePasswort();
      }

      private void txtPasswortWdh_PasswordChanged(object sender, RoutedEventArgs e)
      {
         ValidatePasswort();
      }
   }
}
