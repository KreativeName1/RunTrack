using System.Windows;
using System.Windows.Input;

namespace Klimalauf
{
   /// <summary>
   /// Interaktionslogik für Einstellungen.xaml
   /// </summary>
   public partial class Einstellungen : Window
   {
      private String firstName;
      private String lastName;

      public Einstellungen(string firstName, string lastName)
      {
         InitializeComponent();

         // Set the ScannerName label with the passed names
         ScannerName.Content = $"{lastName}, {firstName}";
         DataContext = this;
         this.firstName = firstName;
         this.lastName = lastName;
      }

      private void LogoutIcon_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Create a new instance of MainWindow
         MainWindow mainWindow = new MainWindow();
         mainWindow.Show();

         // Close the current Scanner window
         this.Close();
      }

      private void Save_Click(object sender, RoutedEventArgs e)
      {

      }

      private void CloseWindow_Click(object sender, RoutedEventArgs e)
      {
         // Open admin panel window
         Scanner adminPanel = new Scanner(firstName, lastName, true);
         adminPanel.Show();
         this.Close();
      }
   }
}
