using Klimalauf.View;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Klimalauf
{
   /// <summary>
   /// Interaktionslogik für Einstellungen.xaml
   /// </summary>
   public partial class Einstellungen : Window
   {
      private MainViewModel _mvmodel;
      private bool _changesMade; // Flagge für Änderungen

      public Einstellungen()
      {
         InitializeComponent();

         _mvmodel = FindResource("mvmodel") as MainViewModel;

         ScannerName.Content = $"{_mvmodel.Benutzer.Vorname}, {_mvmodel.Benutzer.Nachname}";
         DataContext = this;

         LoadContent();
      }

      private void LoadContent()
      {
         using (var db = new LaufDBContext())
         {
            var sortedRundenArten = db.RundenArten.OrderBy(r => r.MaxScanIntervalInSekunden).ToList();
            int rowIndex = 0;

            foreach (RundenArt rundenArt in sortedRundenArten)
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
               integerUpDown.ValueChanged += IntegerUpDown_ValueChanged;

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

               // Löschen
               Button deleteButton = new Button
               {
                  Content = "🗑️",
                  FontSize = 12,
                  Width = 25,
                  Height = 25,
                  VerticalAlignment = VerticalAlignment.Center,
                  HorizontalAlignment = HorizontalAlignment.Left,
                  Margin = new Thickness(30, 3, 5, 5),
                  Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4")),
                  ToolTip = "Löschen",
                  Tag = rundenArt.Name
               };
               deleteButton.Click += DeleteButton_Click;

               Grid.SetRow(deleteButton, rowIndex);
               Grid.SetColumn(deleteButton, 4);
               GridSettings.Children.Add(deleteButton);

               // Einstellungen
               Button optionsButton = new Button
               {
                  Content = "⚙️",
                  FontSize = 12,
                  Width = 25,
                  Height = 25,
                  VerticalAlignment = VerticalAlignment.Center,
                  HorizontalAlignment = HorizontalAlignment.Left,
                  Margin = new Thickness(0, 3, 10, 5),
                  Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4")),
                  ToolTip = "Einstellungen",
                  Tag = rundenArt.Name
               };
               optionsButton.Click += OptionsButton_Click;

               Grid.SetRow(optionsButton, rowIndex);
               Grid.SetColumn(optionsButton, 5);
               GridSettings.Children.Add(optionsButton);

               rowIndex++;
            }

            GridSettings.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Add Rectangle
            Rectangle rect = new Rectangle
            {
               Height = 1,
               Fill = new SolidColorBrush(Colors.Gray),
               Margin = new Thickness(0, 5, 0, 30)
            };

            Grid.SetRow(rect, rowIndex);
            Grid.SetColumnSpan(rect, 6);
            GridSettings.Children.Add(rect);

            rowIndex++;

            Button addButton = new Button
            {
               Content = "➕",
               FontSize = 12,
               Width = 25,
               Height = 25,
               VerticalAlignment = VerticalAlignment.Center,
               HorizontalAlignment = HorizontalAlignment.Center,
               Margin = new Thickness(0, 20, 0, 0),
               ToolTip = "Neue Rundenart",
               Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4"))
            };
            addButton.Click += AddButton_Click;

            Grid.SetRow(addButton, rowIndex);
            Grid.SetColumnSpan(addButton, 6);
            GridSettings.Children.Add(addButton);
         }
      }


      private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
      {
         // Wenn sich der Wert ändert, setze die Flagge auf true
         _changesMade = true;
      }

      private void AddButton_Click(object sender, RoutedEventArgs e)
      {
         VerwaltungRunden verwaltungRunden = new VerwaltungRunden(DialogMode.Neu);
         verwaltungRunden.ShowDialog();
         RefreshGridSettings();
      }

      private void OptionsButton_Click(object sender, RoutedEventArgs e)
      {
         Button optionsButton = sender as Button;
         string rundenartName = optionsButton?.Tag as string;

         if (!string.IsNullOrEmpty(rundenartName))
         {
            using (var db = new LaufDBContext())
            {
               var rundenArt = db.RundenArten.FirstOrDefault(r => r.Name == rundenartName);
               if (rundenArt != null)
               {
                  VerwaltungRunden verwaltungRunden = new VerwaltungRunden(DialogMode.Bearbeiten, rundenArt);
                  verwaltungRunden.ShowDialog();
                  RefreshGridSettings();
               }
               else
               {
                  System.Windows.MessageBox.Show($"Rundenart '{rundenartName}' nicht gefunden", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
               }
            }
         }
      }

      private void DeleteButton_Click(object sender, RoutedEventArgs e)
      {
         Button deleteButton = sender as Button;
         if (deleteButton != null)
         {
            string rundenartName = deleteButton.Tag as string;
            if (!string.IsNullOrEmpty(rundenartName))
            {
               DeleteRundenart(rundenartName);
            }
         }
      }

      private void DeleteRundenart(string rundenartName)
      {
         if (System.Windows.MessageBox.Show("Wollen Sie wirklich " + rundenartName + " löschen?", "Löschne", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
         {
            using (var db = new LaufDBContext())
            {
               var rundenart = db.RundenArten.FirstOrDefault(r => r.Name == rundenartName);
               if (rundenart != null)
               {
                  db.RundenArten.Remove(rundenart);
                  db.SaveChanges();
               }
            }
         }

         RefreshGridSettings();
      }

      private void RefreshGridSettings()
      {
         // Logik zum Aktualisieren fehlt hier noch
         GridSettings.Children.Clear();
         GridSettings.RowDefinitions.Clear();

         LoadContent();
         _changesMade = false; // Änderungen zurücksetzen
      }

      private void LogoutIcon_MouseDown(object sender, MouseButtonEventArgs e)
      {
         MainWindow mainWindow = new MainWindow();
         mainWindow.Show();
         this.Close();
      }

      private void Save_Click(object sender, RoutedEventArgs e)
      {
         if (_changesMade)
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
         else
         {
            System.Windows.MessageBox.Show("Keine Änderungen zum Speichern gefunden", "Keine Änderungen", MessageBoxButton.OK, MessageBoxImage.Information);
         }
      }

      private void CloseWindow_Click(object sender, RoutedEventArgs e)
      {
         Scanner adminPanel = new Scanner();
         adminPanel.Show();
         this.Close();
      }
   }
}
