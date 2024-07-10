using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Klimalauf
{
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();

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
         FirstNameTextBox.Foreground = new SolidColorBrush(Colors.Gray);
         LastNameTextBox.Foreground = new SolidColorBrush(Colors.Gray);
      }

      private void LoginButton_Click(object sender, RoutedEventArgs e)
      {
         string firstName = FirstNameTextBox.Text;
         string lastName = LastNameTextBox.Text;

         // Check if first name and last name are provided
         if (ValidateInputs())
         {
            // Check admin password
            string adminPassword = AdminPasswordBox.Password;
            bool isAdmin = false;
            using (var db = new LaufDBContext())
            {
               var admin = db.Benutzer.FirstOrDefault(b => b.Vorname == firstName && b.Nachname == lastName);
               if (admin != null && BCrypt.Net.BCrypt.Verify(adminPassword, admin.Passwort))
               {
                  isAdmin = true;
               }
            }

            Scanner scanner = new Scanner(firstName, lastName, isAdmin);
            scanner.Show();
            this.Close();
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

      private void SetInvalidInputStyle(TextBox textBox)
      {
         textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C1121C"));
         textBox.Foreground = new SolidColorBrush(Colors.White);
      }

      private void SetValidInputStyle(TextBox textBox)
      {
         textBox.Background = new SolidColorBrush(Colors.White);
         textBox.Foreground = new SolidColorBrush(Colors.Blue);
      }

      private void FirstNameTextBox_GotFocus(object sender, RoutedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         if (textBox.Text == "Max")
         {
            textBox.Text = "";
            textBox.Foreground = new SolidColorBrush(Colors.Blue);
         }
         SetValidInputStyle(textBox);
      }

      private void FirstNameTextBox_LostFocus(object sender, RoutedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         if (string.IsNullOrWhiteSpace(textBox.Text))
         {
            textBox.Text = "Max";
            textBox.Foreground = new SolidColorBrush(Colors.Gray);
         }
         ValidateFirstName();
      }

      private void LastNameTextBox_GotFocus(object sender, RoutedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         if (textBox.Text == "Mustermann")
         {
            textBox.Text = "";
            textBox.Foreground = new SolidColorBrush(Colors.Blue);
         }
         SetValidInputStyle(textBox);
      }

      private void LastNameTextBox_LostFocus(object sender, RoutedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         if (string.IsNullOrWhiteSpace(textBox.Text))
         {
            textBox.Text = "Mustermann";
            textBox.Foreground = new SolidColorBrush(Colors.Gray);
         }
         ValidateLastName();
      }

      private void TextBox_KeyDown(object sender, KeyEventArgs e)
      {
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
               LoginButton_Click(sender, e);
            }
         }
      }

      private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         textBox.Background = new SolidColorBrush(Colors.White);
         textBox.Foreground = new SolidColorBrush(Colors.Blue);
      }

      private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         TextBox textBox = (TextBox)sender;
         textBox.Background = new SolidColorBrush(Colors.White);
         textBox.Foreground = new SolidColorBrush(Colors.Blue);
      }
   }
}
