using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Datenuebersicht.xaml
    /// </summary>
    public partial class Datenuebersicht : Window
    {
        DatenuebersichtModel dumodel;
        private string firstName;
        private string lastName;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dumodel = FindResource("dumodel") as DatenuebersichtModel;
            LoadData();
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
            Scanner adminPanel = new Scanner(firstName, lastName, true);
            adminPanel.Show();
            this.Close();
        }

        private void btnBarcodes_Click(object sender, RoutedEventArgs e)
        {
            // Logik für Barcode-Button
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            // Logik für Download-Button
        }

        private void btnStartseite_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible);

            using (var db = new LaufDBContext())
            {
                dumodel.LstSchule = new ObservableCollection<Schule>(db.Schulen.ToList());
            }
        }

        private void btnRunden_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);

            using (var db = new LaufDBContext())
            {
                dumodel.LstRunde = new ObservableCollection<Runde>(db.Runden.Include(r => r.Schueler).ThenInclude(s => s.Klasse).ThenInclude(k => k.Schule) .Include(s => s.Schueler.Klasse) .ThenInclude(r => r.RundenArt) .ToList());
            }
        }

        private void btnSchueler_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed);

            using (var db = new LaufDBContext())
            {
                dumodel.LstSchueler = new ObservableCollection<Schueler>(db.Schueler
                    .Include(s => s.Klasse)
                        .ThenInclude(k => k.Schule)
                    .Include(s => s.Klasse)
                        .ThenInclude(r => r.RundenArt)
                    .Include(s => s.Runden)
                    .ToList());
            }
        }

        private void btnKlassen_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);

            using (var db = new LaufDBContext())
            {
                var klassenMitDetails = db.Klassen
                                          .Include(k => k.Schule)
                                          .Include(k => k.Schueler)
                                          .ThenInclude(s => s.Runden)
                                          .ToList();

                dumodel.LstKlasse = new ObservableCollection<Klasse>(klassenMitDetails);

                if (dumodel.LstKlasse.Any())
                {
                    var ersteSchule = dumodel.LstKlasse.First().Schule;
                    if (ersteSchule != null)
                    {
                        var schuleId = ersteSchule.Id;
                        Console.WriteLine($"Die ID der ersten Schule ist: {schuleId}");
                    }
                }
            }
        }

        private void btnSchule_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility(Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);

            using (var db = new LaufDBContext())
            {
                dumodel.LstSchule = new ObservableCollection<Schule>(db.Schulen.ToList());
            }
        }

        private void SetVisibility(Visibility schuleGrid, Visibility klasseGrid, Visibility schuelerGrid, Visibility rundenGrid, Visibility startseiteGrid)
        {
            SchuleGrid.Visibility = schuleGrid;
            KlasseGrid.Visibility = klasseGrid;
            SchuelerGrid.Visibility = schuelerGrid;
            RundenGrid.Visibility = rundenGrid;
            StartseiteGrid.Visibility = startseiteGrid;

            btnSchule.Visibility = schuleGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnSchuleDisabled.Visibility = schuleGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnKlassen.Visibility = klasseGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnKlassenDisabled.Visibility = klasseGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnSchueler.Visibility = schuelerGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnSchuelerDisabled.Visibility = schuelerGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnRunden.Visibility = rundenGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnRundenDisabled.Visibility = rundenGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnStartseite.Visibility = startseiteGrid == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            btnStartseiteDisabled.Visibility = startseiteGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            btnBarcodes.Visibility = schuelerGrid == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadData()
        {
            using (var db = new LaufDBContext())
            {
                dumodel.LstSchule = new ObservableCollection<Schule>(db.Schulen.ToList());
                dumodel.LstKlasse = new ObservableCollection<Klasse>(db.Klassen.Include(k => k.Schule).Include(k => k.Schueler).ThenInclude(s => s.Runden).ToList());
                dumodel.LstSchueler = new ObservableCollection<Schueler>(db.Schueler.Include(s => s.Klasse).ThenInclude(k => k.Schule).Include(s => s.Runden).ToList());
                dumodel.LstRunde = new ObservableCollection<Runde>(db.Runden.Include(r => r.Schueler).ThenInclude(s => s.Klasse).ThenInclude(k => k.Schule).ToList());
            }
        }
    }
}
