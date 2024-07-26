using Klimalauf.View;
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
        public TopBar()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {

            // Get the parent Window of the ContentControl
            Window currentWindow = Window.GetWindow(this);

            // Create a new instance of MainWindow
            MainWindow mainWindow = new MainWindow();

            // Show the new MainWindow
            mainWindow.Show();

            // Close the current window (ContentControl's parent)
            currentWindow.Close();

        }

        private void Credits_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Credits credits = new Credits();
            credits.Show();
        }
    }
}
