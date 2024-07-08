using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Klimalauf
{
   /// <summary>
   /// Interaktionslogik für Scanner.xaml
   /// </summary>
   public partial class Scanner : Window
   {
      private MainViewModel mvmodel;

      private StringBuilder barcodeInput = new StringBuilder();

      public Scanner(string firstName, string lastName)
      {
         InitializeComponent();

         // Set the ScannerName label with the passed names
         ScannerName.Content = $"{lastName}, {firstName}";
         DataContext = this;
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         this.mvmodel = FindResource("mvmodel") as MainViewModel;
      }

      private void LogoutButton_Click(object sender, RoutedEventArgs e)
      {
         // Create a new instance of MainWindow
         MainWindow mainWindow = new MainWindow();
         mainWindow.Show();

         // Close the current Scanner window
         this.Close();
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
         //mvmodel.LstScanner.Add(new ScanItem(id));
         using (var db = new LaufDBContext())
            {
                Schueler schueler = db.Schueler.Find(id);
                if (schueler == null)
                {
                    // Zeigt eine Fehlermeldung an, wenn der Schüler nicht gefunden wurde
                    // TODO
                }
                else
                {
                    Runde runde = new Runde();
                    runde.Schueler = schueler;
                    runde.Zeitstempel = DateTime.Now;
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

      
   }
}

