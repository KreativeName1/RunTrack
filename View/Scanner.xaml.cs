using System.Data.Entity;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Klimalauf
{
    public partial class Scanner : Window
    {
        private MainViewModel _mvmodel;
        private StringBuilder barcodeInput = new StringBuilder();
        private DispatcherTimer timer;
        private DateTime lastKeystroke = DateTime.Now;
        private const int scannerInputThreshold = 50;


        public Scanner()
        {
            InitializeComponent();

            DataContext = this;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            HideStatusBoxes();
            timer.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._mvmodel = FindResource("mvmodel") as MainViewModel;
            ScannerName.Content = $"{_mvmodel.Benutzer.Vorname}, {_mvmodel.Benutzer.Nachname}";
            if (_mvmodel.Benutzer.IsAdmin)
            {
                this.lblAdmin.Visibility = Visibility.Visible;
                this.borderAdmin.Visibility = Visibility.Visible;

                this.userName.Visibility = Visibility.Visible;
                this.rectUser.Visibility = Visibility.Visible;

                lstlastScan.Margin = new Thickness(lstlastScan.Margin.Left, lstlastScan.Margin.Top, lstlastScan.Margin.Right, 100);

                btnUebersicht.Click += (sender, e) =>
                {
                    Datenuebersicht datenPanel = new Datenuebersicht();
                    datenPanel.Show();
                    this.Close();
                };

                btnEinstellung.Click += (sender, e) =>
                {
                    Einstellungen optionsPanel = new Einstellungen();
                    optionsPanel.Show();
                    this.Close();
                };

                btnDateien.Click += (sender, e) =>
                {
                    Dateiverwaltung dataPanel = new Dateiverwaltung();
                    dataPanel.Show();
                    this.Close();
                };
            }
            else
            {
                this.lblAdmin.Visibility = Visibility.Hidden;
                this.borderAdmin.Visibility = Visibility.Hidden;

                this.userName.Visibility = Visibility.Visible;
                this.rectUser.Visibility = Visibility.Visible;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void LogoutIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void AddScannedData(int id)
        {
            using (var db = new LaufDBContext())
            {
                Schueler schueler = db.Schueler.FirstOrDefault(s => s.Id == id);

                // Interval in Sekunden holen
                Klasse klasse = db.Klassen.FirstOrDefault(k => k.Schueler.Contains(schueler));
                RundenArt rundenArt = db.RundenArten.Find(klasse.RundenArtId);
                List<Runde> runden  = db.Runden.Where(r => r.SchuelerId == schueler.Id).ToList();
                int IntervalInSekunden = rundenArt.MaxScanIntervalInSekunden;

                bool isSchuelerAlreadyScanned = false;
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


                // check if Schueler with given ID exists
                if (schueler == null)
                {
                    if (isSchuelerAlreadyScanned) Fehlermeldung.Content = $"Der Schüler wurde bereits innerhalb von {IntervalInSekunden} Sekunden eingescannt. ";
                    else Fehlermeldung.Content = "Schüler mit dieser ID existiert nicht.";

                    this.BoxTrue.Visibility = Visibility.Collapsed;
                    this.BoxFalse.Visibility = Visibility.Visible;

                    // Start fade-in animation for BoxFalse
                    Storyboard sb = FindResource("ShowBoxFalse") as Storyboard;
                    sb.Begin(BoxFalse);

                    StartHideTimer();
                }
                else
                {
                    this.BoxFalse.Visibility = Visibility.Collapsed;
                    this.BoxTrue.Visibility = Visibility.Visible;

                    // Start fade-in animation for BoxTrue
                    Storyboard sb = FindResource("ShowBoxTrue") as Storyboard;
                    sb.Begin(BoxTrue);

                    Runde runde = new Runde();
                    runde.Schueler = schueler;
                    runde.Zeitstempel = DateTime.Now;
                    runde.BenutzerName = $"{_mvmodel.Benutzer.Vorname} {_mvmodel.Benutzer.Nachname}";
                    db.Runden.Add(runde);
                    db.SaveChanges();
                    _mvmodel.hinzufügeLetzteRunde(runde);
                    _mvmodel.LstRunden.Add(runde);
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
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            TimeSpan timeSinceLastKeystroke = DateTime.Now - lastKeystroke;
            lastKeystroke = DateTime.Now;

            if (timeSinceLastKeystroke.TotalMilliseconds > scannerInputThreshold)
            {
                barcodeInput.Clear();
            }

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
    }
}
