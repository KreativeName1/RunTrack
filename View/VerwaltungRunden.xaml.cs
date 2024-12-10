using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace RunTrack
{
    // Definiert den Dialogmodus (Neu oder Bearbeiten)
    public enum DialogMode { Neu, Bearbeiten };

    // Teilklasse der VerwaltungRunden-Seite
    public partial class VerwaltungRunden : Page
    {
        // Private Felder für den Dialogmodus, die RundenArt und das Hauptmodell
        private DialogMode mode;
        private RundenArt? rundenArt;
        private MainModel? _pmodel;

        // Private Felder für die ursprünglichen Werte und den Speichern-Status
        private string? originalBezeichnung;
        private int? originalLength;
        private int? originalDauer;
        private bool isSaveClicked = false;

        // Konstruktor, der den Dialogmodus und die RundenArt initialisiert
        public VerwaltungRunden(DialogMode mode, RundenArt? rundenArt = null)
        {
            InitializeComponent();
            _pmodel = FindResource("pmodel") as MainModel ?? new();
            this.Loaded += VerwaltungRunden_Loaded;

            this.mode = mode;
            this.rundenArt = rundenArt;
            SetDialogMode();

            btnCancel.IsTabStop = false;
            btnSave.IsTabStop = false;
        }

        // Setzt den Dialogmodus und initialisiert die Felder entsprechend
        private void SetDialogMode()
        {
            if (mode == DialogMode.Neu)
            {
                this.Title = "Hinzufügen";
            }
            else if (mode == DialogMode.Bearbeiten && rundenArt != null)
            {
                this.Title = "Bearbeiten";

                // Setzt die Textboxen auf die Werte der übergebenen RundenArt
                this.BezeichnungTextBox.Text = rundenArt.Name;
                ((IntegerUpDown)this.FindName("txtLength")).Value = rundenArt.LaengeInMeter;
                ((IntegerUpDown)this.FindName("txtDauer")).Value = rundenArt.MaxScanIntervalInSekunden;
            }
        }

        // Event-Handler für das Laden der Seite
        private void VerwaltungRunden_Loaded(object sender, RoutedEventArgs e)
        {
            // Speichert die ursprünglichen Werte der Felder
            originalBezeichnung = ((TextBox)this.FindName("BezeichnungTextBox")).Text;
            originalLength = ((IntegerUpDown)this.FindName("txtLength")).Value ?? 0;
            originalDauer = ((IntegerUpDown)this.FindName("txtDauer")).Value ?? 0;
        }

        // Event-Handler für den Abbrechen-Button
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Überprüft, ob Daten geändert wurden und zeigt eine Warnung an, falls nicht gespeichert wurde
            if (IsDataChanged())
            {
                if (new Popup().Display("Sie haben nicht gespeichert!", "Wirklich Beenden?", PopupType.Warning, PopupButtons.YesNo) == false) return;
            }

            // Navigiert zur vorherigen Seite
            _pmodel?.Navigate(_pmodel.History[^1]);
        }

        // Überprüft, ob die Daten geändert wurden
        private bool IsDataChanged()
        {
            var currentBezeichnung = ((TextBox)this.FindName("BezeichnungTextBox")).Text;
            var currentLength = ((IntegerUpDown)this.FindName("txtLength")).Value ?? 0;
            var currentDauer = ((IntegerUpDown)this.FindName("txtDauer")).Value ?? 0;

            return currentBezeichnung != originalBezeichnung || currentLength != originalLength || currentDauer != originalDauer;
        }

        // Event-Handler für den Speichern-Button
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Überprüft, ob das Textfeld leer ist
            string inputName = BezeichnungTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputName))
            {
                BezeichnungTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                new Popup().Display("Fehler", "Das Feld 'Bezeichnung' darf nicht leer sein.", PopupType.Error, PopupButtons.Ok);
                return;
            }

            // Überprüft, ob der Name nur erlaubte Zeichen enthält
            Regex regex = new(@"^[a-zA-ZäöüÄÖÜß][a-zA-ZäöüÄÖÜß0-9 ]*$");
            if (!regex.IsMatch(inputName))
            {
                BezeichnungTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                new Popup().Display("Fehler", "Das Feld 'Bezeichnung' darf nur Buchstaben, Zahlen und Leerzeichen enthalten. Zahlen sind nicht am Anfang erlaubt.", PopupType.Error, PopupButtons.Ok);
                return;
            }

            isSaveClicked = true;

            // Speichert die Daten in der Datenbank
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
                    new Popup().Display("Erfolgreich", "Neue Rundenart wurde erfolgreich hinzugefügt.", PopupType.Success, PopupButtons.Ok);
                }
                else if (mode == DialogMode.Bearbeiten && rundenArt != null)
                {
                    rundenArt.Name = inputName;
                    rundenArt.LaengeInMeter = txtLength.Value ?? 0;
                    rundenArt.MaxScanIntervalInSekunden = txtDauer.Value ?? 0;

                    db.RundenArten.Update(rundenArt);
                    db.SaveChanges();

                    new Popup().Display("Erfolgreich", "Rundenart wurde erfolgreich aktualisiert.", PopupType.Success, PopupButtons.Ok);
                }
            }
            ((Einstellungen)_pmodel.History[^1]).RefreshGridSettings();
            _pmodel?.Navigate(_pmodel.History[^1]);
        }

        // Event-Handler für das Schließen des Fensters
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isSaveClicked && IsDataChanged())
            {
                if (new Popup().Display("Sie haben nicht gespeichert!", "Wirklich Beenden?", PopupType.Warning, PopupButtons.YesNo) == true) e.Cancel = true;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BezeichnungTextBox.Focus();
        }
    }
}
