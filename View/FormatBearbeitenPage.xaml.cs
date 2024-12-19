using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    public partial class FormatBearbeitenPage : Page
    {
        private MainModel? _model;
        private PDFEditorModel _pemodel;

        public FormatBearbeitenPage(Format format)
        {
            InitializeComponent();
            if (format == null)
            {
                new Popup().Display("Fehler", "Kein gültiges Format zum Bearbeiten ausgewählt.", PopupType.Error, PopupButtons.Ok);
                _model?.Navigate(_model.History[^1], false);
                return;
            }

            _model = FindResource("pmodel") as MainModel ?? new MainModel();
            _pemodel = new PDFEditorModel();
            _pemodel.LoadData();
            _pemodel.Format = format;
            DataContext = _pemodel;

            debug.Content = "Format: \t" + format.Name;

            LoadDataFormat();

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

            btnNeuladen.Click += (s, e) => _pemodel.AktualisierePDF();
            cbNeueSeite.Unchecked += (s, e) => _pemodel.AktualisierePDF();
            cbNeueSeite.Checked += (s, e) => _pemodel.AktualisierePDF();
            cbBlattgroesse.SelectionChanged += (s, e) => _pemodel.AktualisierePDF();
            cbOrientierung.SelectionChanged += (s, e) => _pemodel.AktualisierePDF();
            cbTyp.SelectionChanged += (s, e) => _pemodel.AktualisierePDF();
        }

        private void LoadDataFormat()
        {
            txtOben.Value = _pemodel.Format.SeitenRandOben;
            txtUnten.Value = _pemodel.Format.SeitenRandUnten;
            txtLinks.Value = _pemodel.Format.SeitenRandLinks;
            txtRechts.Value = _pemodel.Format.SeitenRandRechts;
            txtAbstandHorizontal.Value = _pemodel.Format.ZellenAbstandHorizontal;
            txtAbstandVertikal.Value = _pemodel.Format.ZellenAbstandVertikal;
            txtBreite.Value = _pemodel.Format.ZellenBreite;
            txtHöhe.Value = _pemodel.Format.ZellenHoehe;
            txtGroesse.Value = _pemodel.Format.SchriftGroesse;
            cbTyp.SelectedItem = _pemodel.Format.SchriftTyp;
            cbBlattgroesse.SelectedIndex = _pemodel.Format.BlattGroesseId - 1;
            cbOrientierung.SelectedItem = _pemodel.Format.Orientierung;
            chkKopf.IsChecked = _pemodel.Format.KopfAnzeigen;
            chkZentriert.IsChecked = _pemodel.Format.Zentriert;
            txtSpalten.Value = _pemodel.Format.SpaltenAnzahl;
            txtZeilenAbstand.Value = _pemodel.Format.ZeilenAbstand;
        }

        private void Speichern_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LaufDBContext())
            {
                var existingFormat = db.Formate.SingleOrDefault(f => f.Id == _pemodel.Format.Id);
                if (existingFormat != null)
                {
                    existingFormat.SeitenRandOben = Convert.ToInt32(txtOben.Value);
                    existingFormat.SeitenRandUnten = Convert.ToInt32(txtUnten.Value);
                    existingFormat.SeitenRandLinks = Convert.ToInt32(txtLinks.Value);
                    existingFormat.SeitenRandRechts = Convert.ToInt32(txtRechts.Value);
                    existingFormat.ZellenAbstandHorizontal = Convert.ToInt32(txtAbstandHorizontal.Value);
                    existingFormat.ZellenAbstandVertikal = Convert.ToInt32(txtAbstandVertikal.Value);
                    existingFormat.ZellenBreite = Convert.ToInt32(txtBreite.Value);
                    existingFormat.ZellenHoehe = Convert.ToInt32(txtHöhe.Value);
                    existingFormat.SchriftGroesse = Convert.ToInt32(txtGroesse.Value);
                    existingFormat.SchriftTyp = (SchriftTyp)cbTyp.SelectedItem;
                    existingFormat.BlattGroesse = (BlattGroesse)cbBlattgroesse.SelectedItem;
                    existingFormat.Orientierung = (Orientierung)cbOrientierung.SelectedItem;
                    existingFormat.KopfAnzeigen = (bool)chkKopf.IsChecked;
                    existingFormat.Zentriert = (bool)chkZentriert.IsChecked;
                    existingFormat.SpaltenAnzahl = Convert.ToInt32(txtSpalten.Value);
                    existingFormat.ZeilenAbstand = Convert.ToDouble(txtZeilenAbstand.Value);

                    db.SaveChanges();
                    new Popup().Display("Erfolg", "Das Format wurde erfolgreich aktualisiert.", PopupType.Success, PopupButtons.Ok);
                }
                else
                {
                    new Popup().Display("Fehler", "Das Format konnte nicht gefunden werden.", PopupType.Error, PopupButtons.Ok);
                }
            }
            _model?.Navigate(_model.History[^1], false);
        }

    }
}
