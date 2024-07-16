using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Klimalauf.View
{
   /// <summary>
   /// Interaktionslogik für Credits.xaml
   /// </summary>
   public partial class Credits : Window
   {
      public Credits()
      {
         InitializeComponent();
      }

      private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
      {
         Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
         e.Handled = true;
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         this.Close();
      }
   }


}
