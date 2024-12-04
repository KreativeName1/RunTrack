using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace RunTrack
{
    public enum DialogMode { Neu, Bearbeiten };

    public partial class VerwaltungRunden : Page
    {
        private DialogMode mode;
        private RundenArt? rundenArt;

        private MainModel? _pmodel;

        private string? originalBezeichnung;
        private int? originalLength;
        private int? originalDauer;
        private bool isSaveClicked = false;

        public VerwaltungRunden(DialogMode mode, RundenArt? rundenArt = null)
        {
            InitializeComponent();
            _pmodel = FindResource("pmodel") as MainModel ?? new();
            this.Loaded += VerwaltungRunden_Loaded;

            this.mode = mode;
            this.rundenArt = rundenArt;
            SetDialogMode();
        }

        private void SetDialogMode()
        {
            if (mode == DialogMode.Neu)
            {
                this.Title = "Hinzufügen";
            }
            else if (mode == DialogMode.Bearbeiten && rundenArt != null)
            {
                this.Title = "Bearbeiten";

                // Set the text of the BezeichnungTextBox to the name of the RundenArt passed in
                this.BezeichnungTextBox.Text = rundenArt.Name;
                ((IntegerUpDown)this.FindName("txtLength")).Value = rundenArt.LaengeInMeter;
                ((IntegerUpDown)this.FindName("txtDauer")).Value = rundenArt.MaxScanIntervalInSekunden;
            }
        }

        private void VerwaltungRunden_Loaded(object sender, RoutedEventArgs e)
        {
            // Speichern der ursprünglichen Werte
            originalBezeichnung = ((TextBox)this.FindName("BezeichnungTextBox")).Text;
            originalLength = ((IntegerUpDown)this.FindName("txtLength")).Value ?? 0;
            originalDauer = ((IntegerUpDown)this.FindName("txtDauer")).Value ?? 0;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (IsDataChanged())
            {
                if (new Popup().Display("Sie haben nicht gespeichert!", "Wirklich Beenden?", PopupType.Warning, PopupButtons.YesNo) == false) return;
            }

            _pmodel?.Navigate(_pmodel.History[^1]);
        }

        private bool IsDataChanged()
        {
            var currentBezeichnung = ((TextBox)this.FindName("BezeichnungTextBox")).Text;
            var currentLength = ((IntegerUpDown)this.FindName("txtLength")).Value ?? 0;
            var currentDauer = ((IntegerUpDown)this.FindName("txtDauer")).Value ?? 0;

            return currentBezeichnung != originalBezeichnung || currentLength != originalLength || currentDauer != originalDauer;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen, ob das Textfeld leer ist
            string inputName = BezeichnungTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputName))
            {
                BezeichnungTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                new Popup().Display("Fehler", "Das Feld 'Bezeichnung' darf nicht leer sein.", PopupType.Error, PopupButtons.Ok);
                return;
            }

            // nur Buchstaben, Zahlen und Leerzeichen erlaubt (Zahlen nicht am Anfang), erlaubt ä,ö,ü,ß,Ä,Ö,Ü
            Regex regex = new(@"^[a-zA-ZäöüÄÖÜß][a-zA-ZäöüÄÖÜß0-9 ]*$");
            if (!regex.IsMatch(inputName))
            {
                BezeichnungTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                new Popup().Display("Fehler", "Das Feld 'Bezeichnung' darf nur Buchstaben, Zahlen und Leerzeichen enthalten. Zahlen sind nicht am Anfang erlaubt.", PopupType.Error, PopupButtons.Ok);
                return;
            }

            isSaveClicked = true;

            using (LaufDBContext db = new())
            {
                string inputNameNormalized = inputName.Replace(" ", "").ToLower();

                if (mode == DialogMode.Neu)
                {
                    var existingRundenArt = db.RundenArten.FirstOrDefault(ra =>
                        ra.Name.Trim().Replace(" ", "").ToLower() == inputNameNormalized);

                    if (existingRundenArt != null)
                    {
                        new Popup().Display("Fehler", "Eine RundenArt mit diesem Namen existiert bereits. Bitte wählen Sie einen anderen Namen.", PopupType.Error, PopupButtons.Ok);
                        return;
                    }

                    RundenArt newRundenArt = new()
                    {
                        Name = inputName,
                        LaengeInMeter = txtLength.Value ?? 0,
                        MaxScanIntervalInSekunden = txtDauer.Value ?? 0
                    };

                    db.RundenArten.Add(newRundenArt);
                    db.SaveChanges();
                    new Popup().Display("Erfolgreich", "Neue RundenArt wurde erfolgreich hinzugefügt.", PopupType.Success, PopupButtons.Ok);
                }
                else if (mode == DialogMode.Bearbeiten && rundenArt != null)
                {
                    rundenArt.Name = inputName;
                    rundenArt.LaengeInMeter = txtLength.Value ?? 0;
                    rundenArt.MaxScanIntervalInSekunden = txtDauer.Value ?? 0;

                    db.RundenArten.Update(rundenArt);
                    db.SaveChanges();

                    new Popup().Display("Erfolgreich", "RundenArt wurde erfolgreich aktualisiert.", PopupType.Success, PopupButtons.Ok);
                }
            }
            ((Einstellungen)_pmodel.History[^1]).RefreshGridSettings();
            _pmodel?.Navigate(_pmodel.History[^1]);
        }







        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isSaveClicked && IsDataChanged())
            {
                if (new Popup().Display("Sie haben nicht gespeichert!", "Wirklich Beenden?", PopupType.Warning, PopupButtons.YesNo) == true) e.Cancel = true;
            }
        }

    }
}
