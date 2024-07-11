using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                foreach (RundenArt rundenArt in db.RundenArten)
                {

                    Label label = new Label
                    {
                        Content = rundenArt.Name,
                        FontSize = 14,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Width = 100,
                        Height = 30,
                    };
                    Rundengroesse.Children.Add(label);

                    IntegerUpDown integerUpDown = new IntegerUpDown
                    {
                        Name = rundenArt.Name.Replace(" ", "_"),
                        Value = rundenArt.MaxScanIntervalInSekunden,
                        Width = 40,
                        Height = 30,
                        Margin = new Thickness(0, 10, 0, 0),
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,

                    };
                    RundengroesseInputs.Children.Add(integerUpDown);
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
                foreach (var item in RundengroesseInputs.Children)
                {
                    if (item is IntegerUpDown)
                    {
                        IntegerUpDown integerUpDown = (IntegerUpDown)item;
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
