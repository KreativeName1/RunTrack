using System.Windows;
using System.Windows.Input;

namespace Klimalauf
{
   /// <summary>
   /// Interaktionslogik für Einstellungen.xaml
   /// </summary>
   public partial class Einstellungen : Window
   {
    private MainViewModel _mvmodel;
      public Einstellungen()
      {
         InitializeComponent();

         _mvmodel = FindResource("mvmodel") as MainViewModel;

         ScannerName.Content = $"{_mvmodel.Benutzer.Vorname}, {_mvmodel.Benutzer.Nachname}";
         DataContext = this;
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
         Scanner adminPanel = new Scanner();
         adminPanel.Show();
         this.Close();
      }
   }
}
