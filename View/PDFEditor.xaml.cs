using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    public partial class PDFEditor : Page
    {
        private PDFEditorModel? _pemodel;
        private MainModel? _model;

        // Konstruktor für Barcodes für Läufer in einer Klasse
        public PDFEditor(List<Klasse> klassen) : base()
        {
            Initialize();
            _pemodel.Klassen = klassen;
        }

        // Konstruktor für Urkunden
        public PDFEditor(List<Urkunde> urkunden) : base()
        {
            Initialize();
            _pemodel.Urkunden = new ObservableCollection<Urkunde>(urkunden);
            // Sichtbarkeit von UI-Elementen anpassen
            lblSpalten.Visibility = Visibility.Collapsed;
            txtSpalten.Visibility = Visibility.Collapsed;
            lblBarcodeGroesse.Visibility = Visibility.Collapsed;
            borBarcodeGroesse.Visibility = Visibility.Collapsed;
            lblBarcodeAbstand.Visibility = Visibility.Collapsed;
            borBarcodeAbstand.Visibility = Visibility.Collapsed;
            lblZeilenAbstand.Visibility = Visibility.Visible;
            txtZeilenAbstand.Visibility = Visibility.Visible;
            spKopf.Visibility = Visibility.Collapsed;
            spZentriert.Visibility = Visibility.Collapsed;
        }

        // Konstruktor für Schülerwertung
        public PDFEditor(List<Schueler> schueler) : base()
        {
            Initialize();
            _pemodel.Schueler = new ObservableCollection<Schueler>(schueler);
            // Entferne alle Kinder-Elemente außer dem ersten aus PanelRight
            for (int i = PanelRight.Children.Count - 1; i > 0; i--) PanelRight.Children.RemoveAt(i);
            SchuelerBewertungPanel.Visibility = Visibility.Visible;
        }

        // Konstruktor für Auswertung
        public PDFEditor(List<object> liste, string wertungArt) : base()
        {
            Initialize();
            _pemodel.AuswertungsArt = wertungArt;
            _pemodel.Liste = new(liste);
            // Sichtbarkeit von UI-Elementen anpassen
            lblSpalten.Visibility = Visibility.Collapsed;
            txtSpalten.Visibility = Visibility.Collapsed;
            lblBarcodeGroesse.Visibility = Visibility.Collapsed;
            borBarcodeGroesse.Visibility = Visibility.Collapsed;
            lblBarcodeAbstand.Visibility = Visibility.Collapsed;
            borBarcodeAbstand.Visibility = Visibility.Collapsed;
        }

        // Konstruktor für Barcodes für Läufer
        public PDFEditor(List<Laeufer> liste)
        {
            Initialize();
            _pemodel.Laeufer = new ObservableCollection<Laeufer>(liste);
        }

        // Initialisierungsmethode
        private void Initialize()
        {
            InitializeComponent();
            _pemodel = FindResource("pemodel") as PDFEditorModel ?? new PDFEditorModel();
            _model = FindResource("pmodel") as MainModel ?? new MainModel();
            _pemodel.LoadData();

            // Event-Handler für den Abbrechen-Button
            btnCancel.Click += (s, e) =>
            {
                _pemodel.Quelle = new Uri("about:blank");
                Task.Run(() =>
                {
                    try
                    {
                        string[] filesToDelete = Directory.GetFiles("./Temp", "*.pdf");
                        foreach (string file in filesToDelete) File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                });

                _model?.Navigate(_model.History[^1], false);
            };

            // Event-Handler für den Speichern-Button
            btnSpeichern.Click += (s, e) => Speichern();

            // Event-Handler für das Laden der Seite
            Loaded += (s, e) => _pemodel.AktualisierePDF();
            btnNeuladen.Click += (s, e) => _pemodel.AktualisierePDF();
            cbNeueSeite.Unchecked += (s, e) => _pemodel.AktualisierePDF();
            cbNeueSeite.Checked += (s, e) => _pemodel.AktualisierePDF();
            cbBlattgroessee.SelectionChanged += (s, e) => _pemodel.AktualisierePDF();
            cbOrientierung.SelectionChanged += (s, e) => _pemodel.AktualisierePDF();
            cbTyp.SelectionChanged += (s, e) => _pemodel.AktualisierePDF();
            cbFormate.SelectionChanged += (s, e) => _pemodel.AktualisierePDF();

            // Speichere die aktuellen Werte der Textfelder und Checkboxen
            string currentOben = txtOben.Text;
            string currentUnten = txtUnten.Text;
            string currentLinks = txtLinks.Text;
            string currentRechts = txtRechts.Text;
            string currentAbstandHorizontal = txtAbstandHorizontal.Text;
            string currentAbstandVertikal = txtAbstandVertikal.Text;
            string currentBreite = txtBreite.Text;
            string currentHöhe = txtHöhe.Text;
            string currentGroesse = txtGroesse.Text;
            string currentSpalten = txtSpalten.Text;
            bool currentKopf = chkKopf.IsChecked ?? false;
            bool currentZentriert = chkZentriert.IsChecked ?? false;

            // Überprüfe auf Änderungen und aktualisiere bei Bedarf
            txtOben.LostFocus += (s, e) =>
            {
                if (txtOben.Text != currentOben)
                {
                    currentOben = txtOben.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtUnten.LostFocus += (s, e) =>
            {
                if (txtUnten.Text != currentUnten)
                {
                    currentUnten = txtUnten.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtLinks.LostFocus += (s, e) =>
            {
                if (txtLinks.Text != currentLinks)
                {
                    currentLinks = txtLinks.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtRechts.LostFocus += (s, e) =>
            {
                if (txtRechts.Text != currentRechts)
                {
                    currentRechts = txtRechts.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtAbstandHorizontal.LostFocus += (s, e) =>
            {
                if (txtAbstandHorizontal.Text != currentAbstandHorizontal)
                {
                    currentAbstandHorizontal = txtAbstandHorizontal.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtAbstandVertikal.LostFocus += (s, e) =>
            {
                if (txtAbstandVertikal.Text != currentAbstandVertikal)
                {
                    currentAbstandVertikal = txtAbstandVertikal.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtBreite.LostFocus += (s, e) =>
            {
                if (txtBreite.Text != currentBreite)
                {
                    currentBreite = txtBreite.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtHöhe.LostFocus += (s, e) =>
            {
                if (txtHöhe.Text != currentHöhe)
                {
                    currentHöhe = txtHöhe.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtGroesse.LostFocus += (s, e) =>
            {
                if (txtGroesse.Text != currentGroesse)
                {
                    currentGroesse = txtGroesse.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            txtSpalten.LostFocus += (s, e) =>
            {
                if (txtSpalten.Text != currentSpalten)
                {
                    currentSpalten = txtSpalten.Text;
                    _pemodel.AktualisierePDF();
                }
            };
            chkKopf.Checked += (s, e) =>
            {
                if (chkKopf.IsChecked != currentKopf)
                {
                    currentKopf = chkKopf.IsChecked ?? false;
                    _pemodel.AktualisierePDF();
                }
            };
            chkKopf.Unchecked += (s, e) =>
            {
                if (chkKopf.IsChecked != currentKopf)
                {
                    currentKopf = chkKopf.IsChecked ?? false;
                    _pemodel.AktualisierePDF();
                }
            };
            chkZentriert.Checked += (s, e) =>
            {
                if (chkZentriert.IsChecked != currentZentriert)
                {
                    currentZentriert = chkZentriert.IsChecked ?? false;
                    _pemodel.AktualisierePDF();
                }
            };
            chkZentriert.Unchecked += (s, e) =>
            {
                if (chkZentriert.IsChecked != currentZentriert)
                {
                    currentZentriert = chkZentriert.IsChecked ?? false;
                    _pemodel.AktualisierePDF();
                }
            };
        }

        // Methode zum Speichern der Daten
        private void Speichern()
        {
            using (var db = new LaufDBContext())
            {
                // Überprüfen, ob die maximale Anzahl von 12 Formaten erreicht ist
                if (db.Formate.Count() >= 12)
                {
                    new Popup().Display("Limit erreicht", "Es können maximal 12 Formate angelegt werden.", PopupType.Warning, PopupButtons.Ok);
                    return;
                }

                if (db.Formate.Any(f => f.Name.ToLower().Trim() == _pemodel.Format.Name.ToLower().Trim()))
                {
                    new Popup().Display("Format existiert bereits", "Das Format existiert bereits, bearbeiten Sie es in der Verwaltung", PopupType.Warning, PopupButtons.Ok);
                }
                else
                {
                    bool? result = new Popup().Display("Neues Format speichern?", "Möchten Sie das Format speichern?", PopupType.Question, PopupButtons.YesNo);
                    if (result == false) return;
                    _pemodel.Format.Id = 0;
                    _pemodel.Format.BlattGroesse = db.BlattGroessen.First(x => x.Name == "A4");
                    _pemodel.Format.BlattGroesseId = _pemodel.Format.BlattGroesse.Id;
                    db.Formate.Add(_pemodel.Format);
                }
                db.SaveChanges();
                cbFormate.ItemsSource = db.Formate.ToList();
            }
        }

        // Event-Handler für den Speichern-Button
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SavePdf();
        }

        // Methode zum Speichern der PDF
        private async void SavePdf()
        {
            // Stelle sicher, dass WebView2 initialisiert wurde
            if (webView.CoreWebView2 != null)
            {
                string filePath = GetSaveFilePath();  // Methode, um den Speicherort auszuwählen
                if (string.IsNullOrEmpty(filePath)) return; // Falls der Benutzer keinen Pfad wählt, abbrechen.

                try
                {
                    // Verwende die CoreWebView2 Instanz und die PrintToPdf-Methode
                    bool result = await webView.CoreWebView2.PrintToPdfAsync(filePath);

                    if (result)
                    {
                        new Popup().Display("Erfolg", "PDF erfolgreich gespeichert!", PopupType.Info, PopupButtons.Ok);
                    }
                    else
                    {
                        new Popup().Display("Fehler", "Fehler beim Speichern der PDF.", PopupType.Error, PopupButtons.Ok);
                    }
                }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler", $"Fehler beim Speichern der PDF: {ex.Message}", PopupType.Error, PopupButtons.Ok);
                }
            }
            else
            {
                new Popup().Display("Fehler", "WebView2 ist noch nicht initialisiert.", PopupType.Error, PopupButtons.Ok);
            }
        }

        // Methode, um den Speicherort für die PDF zu wählen
        private string GetSaveFilePath()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF-Dateien (*.pdf)|*.pdf",
                DefaultExt = ".pdf",
                FileName = "Dokument.pdf"
            };

            bool? result = saveFileDialog.ShowDialog();
            return result == true ? saveFileDialog.FileName : string.Empty;
        }

        // Event-Handler für den Drucken-Button
        //private async void btnPrint_Click(object sender, RoutedEventArgs e)
        //{
        //    Window.GetWindow(this).WindowState = WindowState.Maximized;

        //    if (webView.CoreWebView2 != null)
        //    {
        //        try
        //        {
        //            webView.CoreWebView2.ShowPrintUI();
        //        }
        //        catch (Exception ex)
        //        {
        //            new Popup().Display("Fehler", $"Fehler beim Drucken der PDF: {ex.Message}", PopupType.Error, PopupButtons.Ok);
        //        }
        //    }
        //    else
        //    {
        //        new Popup().Display("Fehler", "WebView2 ist noch nicht initialisiert.", PopupType.Error, PopupButtons.Ok);
        //    }
        //}

        // Event-Handler für den Drucken-Button
        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CoreWebView2 != null)
            {
                try
                {
                    // Navigiere zur PrintPreviewPage und übergebe die CoreWebView2-Instanz
                    _model?.Navigate(new PrintPreviewPage(webView.CoreWebView2), false);
                }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler", $"Fehler beim Öffnen der Druckvorschau: {ex.Message}", PopupType.Error, PopupButtons.Ok);
                }
            }
            else
            {
                new Popup().Display("Fehler", "WebView2 ist noch nicht initialisiert.", PopupType.Error, PopupButtons.Ok);
            }
        }


        private void btnFormateVerwalten_Click(object sender, RoutedEventArgs e)
        {
            bool? res = new Popup().Display("Warnung", "Wenn sie auf die Seite gehen, verlassen Sie den PDF Editor und müssen diesen neu aufrufen", PopupType.Warning, PopupButtons.OkCancel);

            // Überprüfe die Antwort des Benutzers
            if (res == true)
            {
                _model?.Navigate(new FormatVerwaltenPage(), false);
            }
        }
    }
}
