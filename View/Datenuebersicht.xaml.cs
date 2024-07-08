using System.Windows;
using System.Windows.Input;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Datenuebersicht.xaml
    /// </summary>
    public partial class Datenuebersicht : Window 
    {
        DatenuebersichtModel dumodel;
        private String firstName;
        private String lastName;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dumodel = FindResource("dumodel") as DatenuebersichtModel;
        }


        public Datenuebersicht(string firstName, string lastName)
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

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            // Open admin panel window
            AdminScanner adminPanel = new AdminScanner(firstName, lastName);
            adminPanel.Show();
            this.Close();
        }

        private void btnBarcodes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStartseite_Click(object sender, RoutedEventArgs e)
        {
            btnSchule.Visibility = Visibility.Visible;
            btnSchuleDisabled.Visibility = Visibility.Collapsed;

            btnKlassen.Visibility = Visibility.Visible;
            btnKlassenDisabled.Visibility = Visibility.Collapsed;

            btnSchueler.Visibility = Visibility.Visible;
            btnSchuelerDisabled.Visibility = Visibility.Collapsed;

            btnRunden.Visibility = Visibility.Visible;
            btnRundenDisabled.Visibility = Visibility.Collapsed;

            btnStartseite.Visibility = Visibility.Collapsed;
            btnStartseiteDisabled.Visibility = Visibility.Visible;


            SchuleGrid.Visibility = Visibility.Collapsed;
            KlasseGrid.Visibility = Visibility.Collapsed;
            SchuelerGrid.Visibility = Visibility.Collapsed;
            RundenGrid.Visibility = Visibility.Collapsed;
            StartseiteGrid.Visibility = Visibility.Visible;

            btnBarcodes.Visibility = Visibility.Collapsed;

        }

        private void btnRunden_Click(object sender, RoutedEventArgs e)
        {
            btnSchule.Visibility = Visibility.Visible;
            btnSchuleDisabled.Visibility = Visibility.Collapsed;

            btnKlassen.Visibility = Visibility.Visible;
            btnKlassenDisabled.Visibility = Visibility.Collapsed;

            btnSchueler.Visibility = Visibility.Visible;
            btnSchuelerDisabled.Visibility = Visibility.Collapsed;

            btnRunden.Visibility = Visibility.Collapsed;
            btnRundenDisabled.Visibility = Visibility.Visible;

            btnStartseite.Visibility = Visibility.Visible;
            btnStartseiteDisabled.Visibility = Visibility.Collapsed;


            SchuleGrid.Visibility = Visibility.Collapsed;
            KlasseGrid.Visibility = Visibility.Collapsed;
            SchuelerGrid.Visibility = Visibility.Collapsed;
            RundenGrid.Visibility = Visibility.Visible;
            StartseiteGrid.Visibility = Visibility.Collapsed;

            btnBarcodes.Visibility = Visibility.Collapsed;
        }

        private void btnSchueler_Click(object sender, RoutedEventArgs e)
        {
            btnSchule.Visibility = Visibility.Visible;
            btnSchuleDisabled.Visibility = Visibility.Collapsed;

            btnKlassen.Visibility = Visibility.Visible;
            btnKlassenDisabled.Visibility = Visibility.Collapsed;

            btnSchueler.Visibility = Visibility.Collapsed;
            btnSchuelerDisabled.Visibility = Visibility.Visible;

            btnRunden.Visibility = Visibility.Visible;
            btnRundenDisabled.Visibility = Visibility.Collapsed;

            btnStartseite.Visibility = Visibility.Visible;
            btnStartseiteDisabled.Visibility = Visibility.Collapsed;


            SchuleGrid.Visibility = Visibility.Collapsed;
            KlasseGrid.Visibility = Visibility.Collapsed;
            SchuelerGrid.Visibility = Visibility.Visible;
            RundenGrid.Visibility = Visibility.Collapsed;
            StartseiteGrid.Visibility = Visibility.Collapsed;

            btnBarcodes.Visibility = Visibility.Visible;
        }

        private void btnKlassen_Click(object sender, RoutedEventArgs e)
        {
            btnSchule.Visibility = Visibility.Visible;
            btnSchuleDisabled.Visibility = Visibility.Collapsed;

            btnKlassen.Visibility = Visibility.Collapsed;
            btnKlassenDisabled.Visibility = Visibility.Visible;

            btnSchueler.Visibility = Visibility.Visible;
            btnSchuelerDisabled.Visibility = Visibility.Collapsed;

            btnRunden.Visibility = Visibility.Visible;
            btnRundenDisabled.Visibility = Visibility.Collapsed;

            btnStartseite.Visibility = Visibility.Visible;
            btnStartseiteDisabled.Visibility = Visibility.Collapsed;


            SchuleGrid.Visibility = Visibility.Collapsed;
            KlasseGrid.Visibility = Visibility.Visible;
            SchuelerGrid.Visibility = Visibility.Collapsed;
            RundenGrid.Visibility = Visibility.Collapsed;
            StartseiteGrid.Visibility = Visibility.Collapsed;

            btnBarcodes.Visibility = Visibility.Collapsed;
        }

        private void btnSchule_Click(object sender, RoutedEventArgs e)
        {
            btnSchule.Visibility = Visibility.Collapsed;
            btnSchuleDisabled.Visibility = Visibility.Visible;

            btnKlassen.Visibility = Visibility.Visible;
            btnKlassenDisabled.Visibility = Visibility.Collapsed;

            btnSchueler.Visibility = Visibility.Visible;
            btnSchuelerDisabled.Visibility = Visibility.Collapsed;

            btnRunden.Visibility = Visibility.Visible;
            btnRundenDisabled.Visibility = Visibility.Collapsed;

            btnStartseite.Visibility = Visibility.Visible;
            btnStartseiteDisabled.Visibility = Visibility.Collapsed;


            SchuleGrid.Visibility = Visibility.Visible;
            KlasseGrid.Visibility = Visibility.Collapsed;
            SchuelerGrid.Visibility = Visibility.Collapsed;
            RundenGrid.Visibility = Visibility.Collapsed;
            StartseiteGrid.Visibility = Visibility.Collapsed;

            btnBarcodes.Visibility = Visibility.Collapsed;



            using (var db = new LaufDBContext())
            {
                // hier zugriff auf die Datenbank

                dumodel.LstSchule = new System.Collections.ObjectModel.ObservableCollection<Schule>(db.Schulen.ToList());

                List<Schule> lstSchulen = db.Schulen.ToList();

            }
        }
        
    }
}
