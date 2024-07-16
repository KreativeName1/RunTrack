using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Klimalauf.View
{
    /// <summary>
    /// Interaktionslogik für VerwaltungRunden.xaml
    /// </summary>
    public partial class VerwaltungRunden : Window
    {
        public VerwaltungRunden()
        {
            InitializeComponent();
        }

      private void txtDauer_GotFocus(object sender, RoutedEventArgs e)
      {
         rectSek.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5fa2e6"));
      }

      private void txtDauer_LostFocus(object sender, RoutedEventArgs e)
      {
         rectSek.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#abadb3"));
      }

      private void txtDauer_MouseEnter(object sender, MouseEventArgs e)
      {
         rectSek.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5fa2e6"));
      }

      private void txtDauer_MouseLeave(object sender, MouseEventArgs e)
      {
         rectSek.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#abadb3"));
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         this.Close();
      }

      private void btnSave_Click(object sender, RoutedEventArgs e)
      {

      }
   }
}
