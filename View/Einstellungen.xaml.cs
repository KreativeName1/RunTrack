using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

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
         using (var db = new LaufDBContext())
         {
            int rowIndex = 0;

            foreach (RundenArt rundenArt in db.RundenArten)
            {
               // Zeilen hinzufügen
               GridSettings.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

               Label label = new Label
               {
                  Content = rundenArt.Name,
                  FontSize = 14,
                  HorizontalAlignment = HorizontalAlignment.Right,
                  VerticalAlignment = VerticalAlignment.Center,
                  Margin = new Thickness(0, 3, 0, 5)
               };
               Grid.SetRow(label, rowIndex);
               Grid.SetColumn(label, 0);
               GridSettings.Children.Add(label);

               Label labelSpacer = new Label
               {
                  Content = "→",
                  FontSize = 14,
                  HorizontalAlignment = HorizontalAlignment.Right,
                  VerticalAlignment = VerticalAlignment.Center,
                  Margin = new Thickness(0, 3, 8, 5)
               };
               Grid.SetRow(labelSpacer, rowIndex);
               Grid.SetColumn(labelSpacer, 1);
               GridSettings.Children.Add(labelSpacer);

               IntegerUpDown integerUpDown = new IntegerUpDown
               {
                  Name = rundenArt.Name.Replace(" ", "_"),
                  Value = rundenArt.MaxScanIntervalInSekunden,
                  Width = 50,
                  Height = 20,
                  VerticalContentAlignment = VerticalAlignment.Center,
                  HorizontalContentAlignment = HorizontalAlignment.Center,
                  Foreground = new SolidColorBrush(Colors.Blue)
               };

               Grid.SetRow(integerUpDown, rowIndex);
               Grid.SetColumn(integerUpDown, 2);
               GridSettings.Children.Add(integerUpDown);

               Label labelSeconds = new Label
               {
                  Content = "Sekunde/n",
                  FontSize = 14,
                  HorizontalAlignment = HorizontalAlignment.Right,
                  VerticalAlignment = VerticalAlignment.Center,
                  Margin = new Thickness(0, 3, 0, 5)
               };
               Grid.SetRow(labelSeconds, rowIndex);
               Grid.SetColumn(labelSeconds, 3);
               GridSettings.Children.Add(labelSeconds);

               rowIndex++;
            }
         }
      }

      private void LogoutIcon_MouseDown(object sender, MouseButtonEventArgs e)
      {
         MainWindow mainWindow = new MainWindow();
         mainWindow.Show();
         this.Close();
      }

      private void Save_Click(object sender, RoutedEventArgs e)
      {
         using (var db = new LaufDBContext())
         {
            foreach (var item in GridSettings.Children)
            {
               if (item is IntegerUpDown integerUpDown)
               {
                  db.RundenArten.First(r => r.Name.Replace(" ", "_") == integerUpDown.Name).MaxScanIntervalInSekunden = integerUpDown.Value ?? 0;
               }
            }
            db.SaveChanges();
         }

         System.Windows.MessageBox.Show("Einstellungen wurden gespeichert", "Einstellungen gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      private void CloseWindow_Click(object sender, RoutedEventArgs e)
      {
         Scanner adminPanel = new Scanner();
         adminPanel.Show();
         this.Close();
      }
   }
}
