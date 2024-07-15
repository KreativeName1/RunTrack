using Klimalauf.View;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Klimalauf
{
   public partial class PDFEditor : Window
   {
      private PDFEditorModel? _pemodel;
      private MainViewModel? _mvmodel;
      private string? pfad;

      public PDFEditor(Klasse klasse) : base()
      {
         InitializeComponent();
         _pemodel = FindResource("pemodel") as PDFEditorModel;
         _mvmodel = FindResource("mvmodel") as MainViewModel;
         _pemodel.Klasse = klasse;
         _pemodel.Format = new Format();

         ScannerName.Content = $"{_mvmodel.Benutzer.Vorname}, {_mvmodel.Benutzer.Nachname}";

         using (var db = new LaufDBContext())
         {
            cbFormate.ItemsSource = db.Formate.ToList();
            cbBlattgroessee.ItemsSource = db.BlattGroessen.ToList();
            cbBlattgroessee.SelectedItem = _pemodel.Format.BlattGroesse ?? db.BlattGroessen.First(x => x.Name == "A4");
            _pemodel.Format.BlattGroesse = cbBlattgroessee.SelectedItem as BlattGroesse;
            cbTyp.ItemsSource = Enum.GetValues(typeof(SchriftTyp));
            cbOrientierung.ItemsSource = Enum.GetValues(typeof(Orientierung));
         }
         cbTyp.SelectedIndex = 0;
         cbOrientierung.SelectedIndex = 0;

         this.Loaded += (s, e) =>
         {
            // Webview mit PDF füllen
            webView.Source = new Uri("about:blank");
            string pfad = PDFGenerator.BarcodesPDF(_pemodel.Klasse, _pemodel.Klasse.Schule.Name, _pemodel.Format);
            webView.Source = new Uri(pfad);

            webView.ZoomFactor = 0.62;
         };



         btnSpeichern.Click += (s, e) =>
         {
            using (var db = new LaufDBContext())
            {
               if (db.Formate.Any(f => f.Name == _pemodel.Format.Name))
               {
                  MessageBoxResult result = MessageBox.Show("Format überschreiben?", "Format existiert bereits", MessageBoxButton.YesNo);
                  if (result == MessageBoxResult.No) return;
                  db.Formate.Update(_pemodel.Format);
               }
               else
               {
                  MessageBoxResult result = MessageBox.Show("Neues Format speichern?", "Format speichern", MessageBoxButton.YesNo);
                  if (result == MessageBoxResult.No) return;
                  _pemodel.Format.Id = 0;
                  _pemodel.Format.BlattGroesse = db.BlattGroessen.First(x => x.Name == "A4");
                  _pemodel.Format.BlattGroesseId = _pemodel.Format.BlattGroesse.Id;
                  db.Formate.Add(_pemodel.Format);
               }
               db.SaveChanges();
               cbFormate.ItemsSource = db.Formate.ToList();
            }
         };

         cbFormate.SelectionChanged += (s, e) =>
         {
            _pemodel.Format = (Format)cbFormate.SelectedItem;
            cbTyp.SelectedIndex = _pemodel.Format.SchriftTyp switch
            {
               SchriftTyp.Normal => 0,
               SchriftTyp.Fett => 1,
               SchriftTyp.Kursiv => 2,
               SchriftTyp.FettKursiv => 3,
               _ => 0
            };
            cbBlattgroessee.SelectedIndex = _pemodel.Format.BlattGroesseId - 1;
            cbOrientierung.SelectedIndex = _pemodel.Format.Orientierung switch
            {
               Orientierung.Hochformat => 0,
               Orientierung.Querformat => 1,
               _ => 0
            };
            AktualisierePDF();
         };

         cbBlattgroessee.SelectionChanged += (s, e) =>
         {
            _pemodel.Format.BlattGroesse = (BlattGroesse)cbBlattgroessee.SelectedItem;
            AktualisierePDF();
         };

         cbTyp.SelectionChanged += (s, e) =>
         {
            _pemodel.Format.SchriftTyp = (SchriftTyp)cbTyp.SelectedIndex;
            AktualisierePDF();
         };

         cbOrientierung.SelectionChanged += (s, e) =>
         {
            _pemodel.Format.Orientierung = (Orientierung)cbOrientierung.SelectedIndex;
            AktualisierePDF();
         };

         // Speichere die aktuellen Werte
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
         txtOben.LostFocus += (s, e) => {
            if (txtOben.Text != currentOben)
            {
               currentOben = txtOben.Text;
               AktualisierePDF();
            }
         };
         txtUnten.LostFocus += (s, e) => {
            if (txtUnten.Text != currentUnten)
            {
               currentUnten = txtUnten.Text;
               AktualisierePDF();
            }
         };
         txtLinks.LostFocus += (s, e) => {
            if (txtLinks.Text != currentLinks)
            {
               currentLinks = txtLinks.Text;
               AktualisierePDF();
            }
         };
         txtRechts.LostFocus += (s, e) => {
            if (txtRechts.Text != currentRechts)
            {
               currentRechts = txtRechts.Text;
               AktualisierePDF();
            }
         };
         txtAbstandHorizontal.LostFocus += (s, e) => {
            if (txtAbstandHorizontal.Text != currentAbstandHorizontal)
            {
               currentAbstandHorizontal = txtAbstandHorizontal.Text;
               AktualisierePDF();
            }
         };
         txtAbstandVertikal.LostFocus += (s, e) => {
            if (txtAbstandVertikal.Text != currentAbstandVertikal)
            {
               currentAbstandVertikal = txtAbstandVertikal.Text;
               AktualisierePDF();
            }
         };
         txtBreite.LostFocus += (s, e) => {
            if (txtBreite.Text != currentBreite)
            {
               currentBreite = txtBreite.Text;
               AktualisierePDF();
            }
         };
         txtHöhe.LostFocus += (s, e) => {
            if (txtHöhe.Text != currentHöhe)
            {
               currentHöhe = txtHöhe.Text;
               AktualisierePDF();
            }
         };
         txtGroesse.LostFocus += (s, e) => {
            if (txtGroesse.Text != currentGroesse)
            {
               currentGroesse = txtGroesse.Text;
               AktualisierePDF();
            }
         };
         txtSpalten.LostFocus += (s, e) => {
            if (txtSpalten.Text != currentSpalten)
            {
               currentSpalten = txtSpalten.Text;
               AktualisierePDF();
            }
         };

         chkKopf.Checked += (s, e) => {
            if (chkKopf.IsChecked != currentKopf)
            {
               currentKopf = chkKopf.IsChecked ?? false;
               AktualisierePDF();
            }
         };
         chkKopf.Unchecked += (s, e) => {
            if (chkKopf.IsChecked != currentKopf)
            {
               currentKopf = chkKopf.IsChecked ?? false;
               AktualisierePDF();
            }
         };
         chkZentriert.Checked += (s, e) => {
            if (chkZentriert.IsChecked != currentZentriert)
            {
               currentZentriert = chkZentriert.IsChecked ?? false;
               AktualisierePDF();
            }
         };
         chkZentriert.Unchecked += (s, e) => {
            if (chkZentriert.IsChecked != currentZentriert)
            {
               currentZentriert = chkZentriert.IsChecked ?? false;
               AktualisierePDF();
            }
         };


         btnNeuladen.Click += (s, e) =>
         {
            AktualisierePDF();
         };
      }

      private void AktualisierePDF()
      {
         webView.Source = new Uri("about:blank");
         string pfad = PDFGenerator.BarcodesPDF(_pemodel.Klasse, _pemodel.Klasse.Schule.Name, _pemodel.Format);
         webView.Source = new Uri(pfad);
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         Datenuebersicht datenuebersicht = new Datenuebersicht();
         datenuebersicht.Show();
         this.Close();

         datenuebersicht.StartseiteGrid.Visibility = Visibility.Collapsed;
         datenuebersicht.btnStartseite.Visibility = Visibility.Visible;
         datenuebersicht.btnStartseiteDisabled.Visibility = Visibility.Collapsed;
         datenuebersicht.KlasseGrid.Visibility = Visibility.Visible;
         datenuebersicht.btnKlassen.Visibility = Visibility.Collapsed;
         datenuebersicht.btnKlassenDisabled.Visibility = Visibility.Visible;
      }

      private void LogoutIcon_MouseDown(object sender, MouseButtonEventArgs e)
      {
         MainWindow mainWindow = new MainWindow();
         mainWindow.Show();
         this.Close();
      }

      //private void btnCredits_Click(object sender, RoutedEventArgs e)
      //{
      //   Credits credits = new Credits();
      //   credits.Show();
      //}
   }
}
