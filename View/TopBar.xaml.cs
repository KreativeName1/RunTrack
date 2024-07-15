using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class TopBar : UserControl
    {
        public string Benutzername { get; set; } = "Vorname, Nachname";

        public TopBar()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).Close();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

        }
    }
}
