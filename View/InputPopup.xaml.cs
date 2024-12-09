using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für PopupWindow.xaml
    /// </summary>
    public partial class InputPopup : MetroWindow
    {
        // Eigenschaft, die das Ergebnis des Popups speichert
        public bool Result { get; set; } = false;

        // Konstruktor, der das Popup mit Titel und Nachricht initialisiert
        public InputPopup(string title, string message)
        {
            InitializeComponent();
            Title = "";
            tbTitel.Content = title;
            tbMessage.Text = message;

            // Event-Handler für den Abbrechen-Button
            btnCancel.Click += (s, e) =>
            {
                Result = false;
                Close();
            };

            // Event-Handler für den OK-Button
            btnOK.Click += (s, e) =>
            {
                Result = true;
                Close();
            };

            // Event-Handler für den Zurück-Button
            //btnBack.Click += (s, e) =>
            //{
            //    Result = false;
            //    Close();
            //};
        }

        // Methode, um den eingegebenen Wert zu erhalten und in den angegebenen Typ zu konvertieren
        public T GetInputValue<T>()
        {
            string value = txtInput.Text;
            if (string.IsNullOrEmpty(value))
            {
                new Popup().Display("Fehler", "Bitte geben Sie einen Wert ein", PopupType.Error, PopupButtons.Ok);
            }
            // Konvertiert den eingegebenen Wert in den angegebenen Typ
            T result = (T)Convert.ChangeType(value, typeof(T));

            if (result == null)
            {
                new Popup().Display("Fehler", "Bitte geben Sie einen gültigen Wert ein", PopupType.Error, PopupButtons.Ok);
            }
            return result;
        }

        // Event-Handler für den Schließen-Button
        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Event-Handler für das MouseEnter-Ereignis des Schließen-Buttons
        private void BTN_Close_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B42041"));
        }

        // Event-Handler für das MouseLeave-Ereignis des Schließen-Buttons
        private void BTN_Close_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        }
    }
}
