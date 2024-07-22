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

         MainWindow mainWindow = new MainWindow();
         mainWindow.Show();
         Window.GetWindow(this).Close();

      }

      private void Credits_MouseDown(object sender, MouseButtonEventArgs e)
      {
         Credits credits = new Credits();
         credits.Show();
      }
   }
}
