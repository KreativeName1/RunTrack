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
        // Private Felder für das Scanner- und Hauptmodell, Barcode-Eingabe und Timer
        private ScannerModel? _smodel;
        private MainModel? _pmodel;
        private StringBuilder barcodeInput = new();
        private DispatcherTimer timer = new();
        private DateTime lastKeystroke = DateTime.Now;
        private const int scannerInputThreshold = 50;

        // Konstruktor der Scanner-Klasse
        public Scanner()
        {
            InitializeComponent();

            // Datenkontext setzen und Modelle initialisieren
            DataContext = this;
            this._smodel = FindResource("smodel") as ScannerModel ?? new();
            this._pmodel = FindResource("pmodel") as MainModel ?? new();

            // Timer initialisieren
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;

            // Event-Handler für Tastatureingaben hinzufügen
            Window window = Application.Current.MainWindow;
            window.PreviewKeyDown += Window_PreviewKeyDown_1;

            // Tab-Stop-Eigenschaften für Steuerelemente setzen
            manualData.IsTabStop = true;
            btnAuswertung.IsTabStop = false;
            btnDateien.IsTabStop = false;
            btnEinstellung.IsTabStop = false;
            btnUebersicht.IsTabStop = false;
            btnAddManual.IsTabStop = false;
        }

        // Event-Handler für den Timer-Tick
        private void Timer_Tick(object? sender, EventArgs e)
        {
            HideStatusBoxes();
            timer.Stop();
        }

        // Event-Handler für das Laden des Fensters
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_smodel == null) return;
            if (_pmodel.Benutzer.IsAdmin)
            {
                // Admin-spezifische Einstellungen
                LoadOverlay.Visibility = Visibility.Hidden;
                this.borderAdmin.Visibility = Visibility.Visible;

                lstlastScan.Margin = new Thickness(lstlastScan.Margin.Left, lstlastScan.Margin.Top, lstlastScan.Margin.Right, 100);

                // Event-Handler für Admin-Buttons hinzufügen
                btnUebersicht.Click += (sender, e) =>
                {
                    LoadOverlay.Visibility = Visibility.Visible;
                    Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _pmodel?.Navigate(new Datenuebersicht());
                            DatenuebersichtModel dumodel = FindResource("dumodel") as DatenuebersichtModel ?? new();
                            dumodel.ReadOnly = false;
                            dumodel.ConnectionString = null;
                        });
                    });
                };

                btnEinstellung.Click += (sender, e) =>
                {
                    LoadOverlay.Visibility = Visibility.Visible;
                    Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _pmodel?.Navigate(new Einstellungen());
                        });
                    });
                };

                btnDateien.Click += (sender, e) =>
                {
                    LoadOverlay.Visibility = Visibility.Visible;
                    Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _pmodel?.Navigate(new Dateiverwaltung());
                        });
                    });
                };
                btnAuswertung.Click += (sender, e) =>
                {
                    LoadOverlay.Visibility = Visibility.Visible;
                    Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _pmodel?.Navigate(new Auswertung());
                        });
                    });
                };
            }
            else
            {
                this.borderAdmin.Visibility = Visibility.Hidden;
            }
        }

        // Event-Handler für den Logout-Button
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _pmodel?.Navigate(new MainWindow());
        }

        // Methode zum Hinzufügen gescannter Daten
        private void AddScannedData(int id, bool isManualInput)
        {
            if (_smodel == null) return;
            using (LaufDBContext db = new())
            {
                Laeufer? laeufer = db.Laeufer.FirstOrDefault(s => s.Id == id);

                bool isLaeuferAlreadyScanned = false;
                int IntervalInSekunden = 0;
                if (laeufer != null)
                {
                    RundenArt? rundenArt;
                    if (laeufer is Schueler)
                    {
                        Schueler schueler = laeufer as Schueler;
                        Klasse klasse = db.Klassen.Find(schueler.KlasseId) ?? new();
                        rundenArt = db.RundenArten.Find(klasse.RundenArtId) ?? new();
                    }
                    else
                    {
                        rundenArt = db.RundenArten.Find(laeufer.RundenArtId) ?? new();
                    }
                    List<Runde> runden = db.Runden.Where(r => r.LaeuferId == laeufer.Id).ToList();
                    IntervalInSekunden = rundenArt.MaxScanIntervalInSekunden;

                    if (runden.Count > 0)
                    {
                        foreach (Runde runde in runden)
                        {
                            TimeSpan difference = DateTime.Now - runde.Zeitstempel;
                            Trace.WriteLine($"Between {DateTime.Now} and {runde.Zeitstempel}: {difference.TotalSeconds}");
                            if (difference.TotalSeconds < IntervalInSekunden)
                            {
                                isLaeuferAlreadyScanned = true;
                                laeufer = null;
                                break;
                            }
                        }
                    }
                }

                if (laeufer == null)
                {
                    if (isLaeuferAlreadyScanned)
                        Fehlermeldung.Content = $"Der Läufer wurde bereits innerhalb von {IntervalInSekunden} Sekunden eingescannt.";
                    else
                        Fehlermeldung.Content = $"Läufer mit der ID {id} existiert nicht.";

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
                    runde.Laeufer = laeufer;
                    runde.Zeitstempel = DateTime.Now;
                    runde.ProgrammKey = UniqueKey.GetKey();
                    runde.BenutzerName = $"{_pmodel?.Benutzer.Vorname.Substring(0, 1).ToUpper()}{_pmodel?.Benutzer.Vorname.Substring(1).ToLower()}_{_pmodel?.Benutzer.Nachname.Substring(0, 1).ToUpper()}{_pmodel?.Benutzer.Nachname.Substring(1).ToLower()}";
                    db.Runden.Add(runde);
                    db.SaveChanges();
                    _smodel.hinzufügeLetzteRunde(runde);
                    _smodel.LstRunden.Add(runde);
                    StartHideTimer();
                }
            }
        }

        // Timer zum Ausblenden der Statusboxen starten
        private void StartHideTimer()
        {
            timer.Stop();
            timer.Start();
        }

        // Statusboxen ausblenden
        private void HideStatusBoxes()
        {
            this.BoxTrue.Visibility = Visibility.Collapsed;
            this.BoxFalse.Visibility = Visibility.Collapsed;
            this.BoxErrorManual.Visibility = Visibility.Collapsed;
        }

        // Event-Handler für Tastatureingaben im Fenster
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

        // Event-Handler für Tastatureingaben im manuellen Eingabefeld
        private void manualData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LogicManualInput();
            }
        }

        // Event-Handler für den Plus-Button
        private void ButtonPlus_Click_1(object sender, RoutedEventArgs e)
        {
            LogicManualInput();
        }

        // Logik für die manuelle Eingabe
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

        // Fehleranzeige für manuelle Eingabe
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

        // Event-Handler für Mausklicks im manuellen Eingabefeld
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
