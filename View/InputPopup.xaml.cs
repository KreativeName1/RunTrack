using System.Windows;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für PopupWindow.xaml
    /// </summary>
    ///

    public partial class InputPopup : Window
    {
        public bool Result { get; set; } = false;
        public InputPopup(string title, string message)
        {
            InitializeComponent();
            Title = title;
            tbTitel.Content = title;
            tbMessage.Text = message;

            btnCancel.Click += (s, e) => {
                Result = false;
                Close();
            };
            btnOK.Click += (s, e) => {
                Result = true;
                Close();
            };
        }

        public T GetInputValue<T>()
        {
            string value = txtInput.Text;
            if (string.IsNullOrEmpty(value)) {
                new Popup().Display("Fehler", "Bitte geben Sie einen Wert ein", PopupType.Error, PopupButtons.Ok);
            }
            // convert
            T result = (T)Convert.ChangeType(value, typeof(T));

            if (result == null) {
                new Popup().Display("Fehler", "Bitte geben Sie einen gültigen Wert ein", PopupType.Error, PopupButtons.Ok);
            }
            return result;
        }

    }
}
