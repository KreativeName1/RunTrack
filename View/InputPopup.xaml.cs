using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für PopupWindow.xaml
    /// </summary>
    ///

    public partial class InputPopup : MetroWindow
    {
        public bool Result { get; set; } = false;
        public InputPopup(string title, string message)
        {
            InitializeComponent();
            Title = "";
            tbTitel.Content = title;
            tbMessage.Text = message;

            btnCancel.Click += (s, e) =>
            {
                Result = false;
                Close();
            };
            btnOK.Click += (s, e) =>
            {
                Result = true;
                Close();
            };

            btnBack.Click += (s, e) =>
            {
                Result = false;

                Close();
            };
        }

        public T GetInputValue<T>()
        {
            string value = txtInput.Text;
            if (string.IsNullOrEmpty(value))
            {
                new Popup().Display("Fehler", "Bitte geben Sie einen Wert ein", PopupType.Error, PopupButtons.Ok);
            }
            // convert
            T result = (T)Convert.ChangeType(value, typeof(T));

            if (result == null)
            {
                new Popup().Display("Fehler", "Bitte geben Sie einen gültigen Wert ein", PopupType.Error, PopupButtons.Ok);
            }
            return result;
        }


        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BTN_Close_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B42041"));
        }
        private void BTN_Close_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        }

    }
}
