using FullControls.Controls;
using Klimalauf.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Klimalauf
{
   public partial class MainWindow : Window
   {
      private MainViewModel _viewModel;

      public MainWindow()
      {
         InitializeComponent();

         _viewModel = FindResource("mvmodel") as MainViewModel;

         // Prüfen, ob Admin existiert. Wenn nicht, AdminErstellen öffnen
         using (var db = new LaufDBContext())
         {
            if (db.Benutzer.Count() == 0)
            {
               AdminErstellen adminErstellen = new AdminErstellen();
               adminErstellen.Show();
               this.Close();
            }
         }
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         FirstNameTextBox.Focus();
         FirstNameTextBox.ForegroundBrush = new SolidColorBrush(Colors.Gray);
         LastNameTextBox.ForegroundBrush = new SolidColorBrush(Colors.Gray);
      }

      private void LoginButton_Click(object sender, RoutedEventArgs e)
      {
         _viewModel.Benutzer = new Benutzer
         {
            Vorname = FirstNameTextBox.Text,
            Nachname = LastNameTextBox.Text,
         };

         // Check if first name and last name are provided
         if (ValidateInputs())
         {
            // Check admin password
            _viewModel.Benutzer.Passwort = AdminPasswordBox.Password;
            using (var db = new LaufDBContext())
            {
               Benutzer? admin = db.Benutzer.FirstOrDefault(b => b.Vorname == _viewModel.Benutzer.Vorname && b.Nachname == _viewModel.Benutzer.Nachname);
               if (admin != null && BCrypt.Net.BCrypt.Verify(_viewModel.Benutzer.Passwort, admin.Passwort))
               {
                  _viewModel.Benutzer.IsAdmin = true;
                  Scanner scanner = new Scanner();
                  scanner.Show();
                  this.Close();
               }
               else
               {
                  // Password does not match, show warning
                  SetInvalidInputStyle(AdminPasswordBox);
                  warningPassword.Visibility = Visibility.Visible;
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

         if (!ValidatePassword())
         {
            isValid = false;
         }

         return isValid;
      }

      private bool ValidateFirstName()
      {
         if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || FirstNameTextBox.Text == "Max")
         {
            SetInvalidInputStyle(FirstNameTextBox);
            return false;
         }
         else
         {
            SetValidInputStyle(FirstNameTextBox);
            return true;
         }
      }

      private bool ValidateLastName()
      {
         if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) || LastNameTextBox.Text == "Mustermann")
         {
            SetInvalidInputStyle(LastNameTextBox);
            return false;
         }
         else
         {
            SetValidInputStyle(LastNameTextBox);
            return true;
         }
      }

      private bool ValidatePassword()
      {
         bool isValid = true;

         // Passwortfelder leeren, wenn das Passwort leer ist
         if (string.IsNullOrEmpty(AdminPasswordBox.Password))
         {
            SetInvalidInputStyle(AdminPasswordBox);
            isValid = false;
         }
         else
         {
            using (var db = new LaufDBContext())
            {
               Benutzer? admin = db.Benutzer.FirstOrDefault(b => b.Vorname == FirstNameTextBox.Text && b.Nachname == LastNameTextBox.Text);
               if (admin != null && BCrypt.Net.BCrypt.Verify(AdminPasswordBox.Password, admin.Passwort))
               {
                  SetValidInputStyle(AdminPasswordBox);
                  warningPassword.Visibility = Visibility.Collapsed; // Hide warning only when password is correct
               }
               else
               {
                  SetInvalidInputStyle(AdminPasswordBox);
                  warningPassword.Visibility = Visibility.Visible; // Show warning if password is incorrect
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
         if (textBox.Name == "FirstNameTextBox")
            warningVorname.Visibility = Visibility.Visible;
         else if (textBox.Name == "LastNameTextBox")
            warningNachname.Visibility = Visibility.Visible;
      }

      private void SetValidInputStyle(TextBoxPlus textBox)
      {
         textBox.UnderlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067c0"));
         textBox.UnderlineThickness = new Thickness(0, 0, 0, 2);

         if (textBox.Name == "FirstNameTextBox")
            warningVorname.Visibility = Visibility.Collapsed;
         else if (textBox.Name == "LastNameTextBox")
            warningNachname.Visibility = Visibility.Collapsed;
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
         TextBox textBox = (TextBox)sender;
         textBox.Background = new SolidColorBrush(Colors.White);
         textBox.Foreground = new SolidColorBrush(Colors.Blue);
         UpdateBorderPasswordVisibility();

         if (!string.IsNullOrEmpty(textBox.Text))
         {
            if (AdminPasswordBox != null)
            {
               AdminPasswordBox.Password = string.Empty;
               warningPassword.Visibility = Visibility.Collapsed;
            }
         }
      }

      private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         textBox.Background = new SolidColorBrush(Colors.White);
         textBox.Foreground = new SolidColorBrush(Colors.Blue);

         if (!string.IsNullOrEmpty(textBox.Text))
         {
            if (AdminPasswordBox != null)
            {
               AdminPasswordBox.Password = string.Empty;
               warningPassword.Visibility = Visibility.Collapsed;
            }
         }

         UpdateBorderPasswordVisibility();
      }

      private void UpdateBorderPasswordVisibility()
      {
         if (FirstNameTextBox != null && LastNameTextBox != null && borderPassword != null)
         {
            using (var db = new LaufDBContext())
            {
               Benutzer user = db.Benutzer.FirstOrDefault(b => b.Vorname == FirstNameTextBox.Text && b.Nachname == LastNameTextBox.Text);
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
                  btnLogin.Margin = new Thickness(btnLogin.Margin.Left, 200, btnLogin.Margin.Right, btnLogin.Margin.Bottom);
               }
            }
         }
      }

      private void btnCredits_Click(object sender, RoutedEventArgs e)
      {
         Credits cr = new Credits();
         cr.ShowDialog();
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
   }
}
