using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace RunTrack
{
   public partial class Scanner : Page
   {
      private ScannerModel? _smodel;
      private MainModel? _pmodel;
      private StringBuilder barcodeInput = new();
      private DispatcherTimer timer = new();
      private DateTime lastKeystroke = DateTime.Now;
      private const int scannerInputThreshold = 50;

      public Scanner()
      {
         InitializeComponent();

         DataContext = this;
         this._smodel = FindResource("smodel") as ScannerModel ?? new();
         this._pmodel = FindResource("pmodel") as MainModel ?? new();

         timer = new DispatcherTimer();
         timer.Interval = TimeSpan.FromSeconds(5);
         timer.Tick += Timer_Tick;

         Window window = Application.Current.MainWindow;
         window.PreviewKeyDown += Window_PreviewKeyDown_1;


      }

      private void Timer_Tick(object? sender, EventArgs e)
      {
         HideStatusBoxes();
         timer.Stop();
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         if (_smodel == null) return;
         if (_pmodel.Benutzer.IsAdmin)
         {
            this.borderAdmin.Visibility = Visibility.Visible;

            lstlastScan.Margin = new Thickness(lstlastScan.Margin.Left, lstlastScan.Margin.Top, lstlastScan.Margin.Right, 100);

            btnUebersicht.Click += (sender, e) =>
            {
               _pmodel?.Navigate(new Datenuebersicht());
            };

            btnEinstellung.Click += (sender, e) =>
            {
               _pmodel?.Navigate(new Einstellungen());
            };

            btnDateien.Click += (sender, e) =>
            {
               _pmodel?.Navigate(new Dateiverwaltung());
            };
            btnAuswertung.Click += (sender, e) =>
            {
               _pmodel?.Navigate(new Auswertung());
            };
         }
         else
         {
            this.borderAdmin.Visibility = Visibility.Hidden;
         }
      }

      private void LogoutButton_Click(object sender, RoutedEventArgs e)
      {
         _pmodel?.Navigate(new MainWindow());
      }
      private void AddScannedData(int id, bool isManualInput)
      {
         if (_smodel == null) return;
         using (LaufDBContext db = new())
         {
            Schueler? schueler = db.Schueler.FirstOrDefault(s => s.Id == id);

            bool isSchuelerAlreadyScanned = false;
            int IntervalInSekunden = 0;
            if (schueler != null)
            {
               Klasse? klasse = db.Klassen.FirstOrDefault(k => k.Schueler.Contains(schueler)) ?? new();
               RundenArt? rundenArt = db.RundenArten.Find(klasse.RundenArtId) ?? new();
               List<Runde> runden = db.Runden.Where(r => r.SchuelerId == schueler.Id).ToList();
               IntervalInSekunden = rundenArt.MaxScanIntervalInSekunden;

               if (runden.Count > 0)
               {
                  foreach (Runde runde in runden)
                  {
                     TimeSpan difference = DateTime.Now - runde.Zeitstempel;
                     Trace.WriteLine($"Between {DateTime.Now} and {runde.Zeitstempel}: {difference.TotalSeconds}");
                     if (difference.TotalSeconds < IntervalInSekunden)
                     {
                        isSchuelerAlreadyScanned = true;
                        schueler = null;
                        break;
                     }
                  }
               }
            }

            if (schueler == null)
            {
               if (isSchuelerAlreadyScanned)
                  Fehlermeldung.Content = $"Der Schüler wurde bereits innerhalb von {IntervalInSekunden} Sekunden eingescannt.";
               else
                  Fehlermeldung.Content = $"Schüler mit der ID {id} existiert nicht.";

               this.BoxTrue.Visibility = Visibility.Collapsed;
               this.BoxFalse.Visibility = Visibility.Visible;
               this.BoxErrorManual.Visibility = Visibility.Collapsed;

               // Fehleranzeige nur bei manueller Eingabe
               if (isManualInput)
               {
                  ErrorEffectManualData(true);  // Fehlerstatus visualisieren
               }


               // Start fade-in animation for BoxFalse
               Storyboard sb = FindResource("ShowBoxFalse") as Storyboard ?? new();
               sb.Begin(BoxFalse);

               StartHideTimer();
            }
            else
            {
               this.BoxFalse.Visibility = Visibility.Collapsed;
               this.BoxTrue.Visibility = Visibility.Visible;
               this.BoxErrorManual.Visibility = Visibility.Collapsed;

               // Start fade-in animation for BoxTrue
               Storyboard sb = FindResource("ShowBoxTrue") as Storyboard ?? new();
               sb.Begin(BoxTrue);

               // Manuelle Eingabe zurücksetzen
               manualData.Text = "";
               ErrorEffectManualData(false);  // Fehleranzeige zurücksetzen

               Runde runde = new();
               runde.Schueler = schueler;
               runde.Zeitstempel = DateTime.Now;
               runde.BenutzerName = $"{_pmodel?.Benutzer.Vorname} {_pmodel?.Benutzer.Nachname}";
               db.Runden.Add(runde);
               db.SaveChanges();
               _smodel.hinzufügeLetzteRunde(runde);
               _smodel.LstRunden.Add(runde);
               StartHideTimer();
            }
         }
      }


      private void StartHideTimer()
      {
         timer.Stop();
         timer.Start();
      }

      private void HideStatusBoxes()
      {
         this.BoxTrue.Visibility = Visibility.Collapsed;
         this.BoxFalse.Visibility = Visibility.Collapsed;
         this.BoxErrorManual.Visibility = Visibility.Collapsed;
      }

      private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
      {
         TimeSpan timeSinceLastKeystroke = DateTime.Now - lastKeystroke;
         lastKeystroke = DateTime.Now;

         if (timeSinceLastKeystroke.TotalMilliseconds > scannerInputThreshold)
         {
            barcodeInput.Clear();
         }

         if (e.Key == Key.Enter || e.Key == Key.Tab)
         {
            string scannedData = barcodeInput.ToString().Trim();
            if (!string.IsNullOrEmpty(scannedData))
            {
               try
               {
                  int scannedDataInt = int.Parse(scannedData);
                  AddScannedData(scannedDataInt, false);  // Kein manueller Input
               }
               catch (FormatException)
               {
                  Console.WriteLine("Ungültige Eingabe. Der String konnte nicht in eine Zahl umgewandelt werden.");
               }
               finally
               {
                  barcodeInput.Clear();
               }
            }
         }
         else
         {
            char c = (char)KeyInterop.VirtualKeyFromKey(e.Key);
            barcodeInput.Append(c);
         }
      }



      private void manualData_KeyDown(object sender, KeyEventArgs e)

      {
         if (e.Key == Key.Enter)
         {
            //int scannedData = _smodel.ManuelleEingabe;
            //Trace.WriteLine($"Scanned Data: {scannedData}");

            //try
            //{
            //    AddScannedData(_smodel.ManuelleEingabe);
            //}
            //catch (FormatException)
            //{
            //    Console.WriteLine("Ungültige Eingabe. Der String konnte nicht in eine Zahl umgewandelt werden.");
            //}
            //finally
            //{
            //}
            LogicManualInput();
         }
      }

      // Funktioniert für den Button
      private void ButtonPlus_Click_1(object sender, RoutedEventArgs e)
      {
         //manualData_KeyDown(sender, new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Enter));
         LogicManualInput();
      }

      private void LogicManualInput()
      {
         if (int.TryParse(manualData.Text, out int scannedDataInt))
         {
            AddScannedData(scannedDataInt, true);
         }
         else
         {
            this.BoxTrue.Visibility = Visibility.Collapsed;
            this.BoxFalse.Visibility = Visibility.Collapsed;
            this.BoxErrorManual.Visibility = Visibility.Visible;

            ErrorEffectManualData(true);  // Fehlerstatus visualisieren

            // Start fade-in animation for BoxErrorManual
            Storyboard sb = FindResource("ShowBoxErrorManual") as Storyboard ?? new();
            sb.Begin(BoxErrorManual);

            StartHideTimer();
         }
      }


      private void ErrorEffectManualData(bool isError)
      {
         if (isError)
         {
            DropShadowEffect shadow = new DropShadowEffect
            {
               Color = Colors.Red,
               BlurRadius = 10,
               ShadowDepth = 0,
               Opacity = 0.8
            };
            manualData.Effect = shadow;
         }
         else
         {
            manualData.Effect = null;
         }
      }

      private void manualData_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         if (!manualData.IsFocused)
         {
            manualData.Focus();
            e.Handled = true;
         }
      }


   }
}
