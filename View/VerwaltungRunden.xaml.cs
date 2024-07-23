using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Klimalauf.View
{
    public enum DialogMode { Neu, Bearbeiten };

    public partial class VerwaltungRunden : Window
    {
        private DialogMode mode;
        private RundenArt? rundenArt;

        private string? originalBezeichnung;
        private int? originalLength;
        private int? originalDauer;
        private bool isSaveClicked = false;

        public VerwaltungRunden(DialogMode mode, RundenArt? rundenArt = null)
        {
            InitializeComponent();
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
                this.operationName.Content = "Hinzufügen";
            }
            else if (mode == DialogMode.Bearbeiten && rundenArt != null)
            {
                this.Title = "Bearbeiten";
                this.operationName.Content = "Bearbeiten";

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
                if (new Popup().Display("Sie haben nicht gespeichert!", "Wirklich Beenden?", PopupType.Warning, PopupButtons.YesNo).Result == false) return;
            }
            this.Close();
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
            isSaveClicked = true;
            using (var db = new LaufDBContext())
            {
                // Normalisierte Version des Eingabenames ohne Leerzeichen am Anfang, Ende und innen
                string inputName = BezeichnungTextBox.Text.Trim();
                string inputNameNormalized = inputName.Replace(" ", "").ToLower();

                if (mode == DialogMode.Neu)
                {
                    // Überprüfen, ob der Name nach der Normalisierung bereits existiert
                    var existingRundenArt = db.RundenArten.FirstOrDefault(ra =>
                        ra.Name.Trim().Replace(" ", "").ToLower() == inputNameNormalized);

                    if (existingRundenArt != null)
                    {
                        // Eintrag mit diesem Namen existiert bereits
                        new Popup().Display("Fehler", "Eine RundenArt mit diesem Namen existiert bereits. Bitte wählen Sie einen anderen Namen.", PopupType.Error, PopupButtons.Ok);
                        return;
                    }

                    // Erstellen einer neuen RundenArt und Hinzufügen zur Datenbank 
                    RundenArt newRundenArt = new RundenArt
                    {
                        Name = inputName, // Hier den bereinigten Namen verwenden
                        LaengeInMeter = (int)txtLength.Value,
                        MaxScanIntervalInSekunden = (int)txtDauer.Value
                    };

                    db.RundenArten.Add(newRundenArt);
                    db.SaveChanges();
                    new Popup().Display("Erfolgreich", "Neue RundenArt wurde erfolgreich hinzugefügt.", PopupType.Success, PopupButtons.Ok);
                }
                else if (mode == DialogMode.Bearbeiten && rundenArt != null)
                {
                    // Bearbeiten einer bestehenden RundenArt
                    rundenArt.Name = inputName; // Hier den bereinigten Namen verwenden
                    rundenArt.LaengeInMeter = (int)txtLength.Value;
                    rundenArt.MaxScanIntervalInSekunden = (int)txtDauer.Value;

                    db.RundenArten.Update(rundenArt);
                    db.SaveChanges();

                    new Popup().Display("Erfolgreich", "RundenArt wurde erfolgreich aktualisiert.", PopupType.Success, PopupButtons.Ok);
                }
            }

            this.Close();
        }






        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isSaveClicked && IsDataChanged())
            {
                PopupResult result = new Popup().Display("Sie haben nicht gespeichert!", "Wirklich Beenden?", PopupType.Warning, PopupButtons.YesNo);
                if (result.Result == true) e.Cancel = true; // Cancel the closing action
            }
        }

    }
}
