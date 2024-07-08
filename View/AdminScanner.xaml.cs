using Org.BouncyCastle.Utilities.Encoders;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Klimalauf
{
   /// <summary>
   /// Interaktionslogik für AdminScanner.xaml
   /// </summary>
   public partial class AdminScanner : Window
   {
      private MainViewModel mvmodel;

      private StringBuilder barcodeInput = new StringBuilder();
      private String firstName;
      private String lastName;

      public AdminScanner(string firstName, string lastName)
      {
         InitializeComponent();

         // Set the ScannerName label with the passed names
         ScannerName.Content = $"{lastName}, {firstName}";
         DataContext = this;
         this.firstName = firstName;
         this.lastName = lastName;
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         this.mvmodel = FindResource("mvmodel") as MainViewModel;
      }

      private void LogoutIcon_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Create a new instance of MainWindow
         MainWindow mainWindow = new MainWindow();
         mainWindow.Show();

         // Close the current Scanner window
         this.Close();
      }


      private void AddScannedData(int id)
      {
            // Beispielhaftes Hinzufügen eines neuen Eintrags. 
            // Die tatsächliche Logik kann je nach Datenformat und Anforderungen variieren.
            using (var db = new LaufDBContext())
            {
                Schueler schueler = db.Schueler.Find(id);
                if (schueler == null)
                {
                    // Zeigt eine Fehlermeldung an, wenn der Schüler nicht gefunden wurde
                    // TODO
                    this.BoxTrue.Visibility = Visibility.Collapsed;
                    this.BoxFalse.Visibility = Visibility.Visible;
                }
                else
                {
                    this.BoxFalse.Visibility = Visibility.Collapsed;
                    this.BoxTrue.Visibility = Visibility.Visible;
                    Runde runde = new Runde();
                    runde.Schueler = schueler;
                    runde.Zeitstempel = DateTime.Now;
                    runde.BenutzerName = $"{firstName} {lastName}";
                    db.Runden.Add(runde);
                    db.SaveChanges();
                    mvmodel.LstRunden.Add(runde);
                }
            }
        }

      private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
      {
         // Überprüfen, ob eine Tastatureingabe von einem Barcode-Scanner stammt (normalerweise durch Enter-Taste beendet)
         if (e.Key == Key.Enter)
         {
            string scannedData = barcodeInput.ToString().Trim();
            if (!string.IsNullOrEmpty(scannedData))
            {
               try
               {
                  int scannedDataInt = int.Parse(scannedData);
                  AddScannedData(scannedDataInt);
               }
               catch (FormatException)
               {
                  // Fehlerbehandlung, falls die Konvertierung fehlschlägt
                  Console.WriteLine("Ungültige Eingabe. Der String konnte nicht in eine Zahl umgewandelt werden.");
               }
               finally
               {
                  // Reset the barcodeInput for the next scan
                  barcodeInput.Clear();
               }
            }
         }
         else
         {
            // Hier wird der gescannte Barcode zusammengesetzt, da der Scanner die Zeichen nacheinander sendet
            char c = (char)KeyInterop.VirtualKeyFromKey(e.Key);
            barcodeInput.Append(c);
         }
      }

      private void btnUebersicht_Click(object sender, RoutedEventArgs e)
      {
         // Open admin panel window
         Datenuebersicht datenPanel = new Datenuebersicht(firstName, lastName);
         datenPanel.Show();
         this.Close();
      }

      private void btnEinstellung_Click(object sender, RoutedEventArgs e)
      {
         // Open admin panel window
         Einstellungen optionsPanel = new Einstellungen(firstName, lastName);
         optionsPanel.Show();
         this.Close();
      }

      private void btnDateien_Click(object sender, RoutedEventArgs e)
      {
         // Open admin panel window
         Dateiverwaltung dataPanel = new Dateiverwaltung(firstName, lastName);
         dataPanel.Show();
         this.Close();
      }
   }
}
