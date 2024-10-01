using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RunTrack
{
   /// <summary>
   /// Interaktionslogik für Main.xaml
   /// </summary>
   public partial class Main : MetroWindow
   {
      private MainModel _pmodel;
      public Main()
      {
         InitializeComponent();

         _pmodel = FindResource("pmodel") as MainModel ?? new();
         MainWindow main = new();
         _pmodel.CurrentPage = main;

         this.PreviewKeyDown += (sender, e) =>
         {
            if (e.Key == Key.F11) ChangeState();
         };



      }

      private void ChangeState()
      {
         if (this.WindowState == WindowState.Maximized)
         {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;
            this.IgnoreTaskbarOnMaximize = false;
            this.ShowTitleBar = true;
         }
         else
         {
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            this.IgnoreTaskbarOnMaximize = true;
            this.ShowTitleBar = false;
         }
      }
      private void BTN_Minimize_Click(object sender, RoutedEventArgs e)
      {
         this.WindowState = WindowState.Minimized;
      }

      private void BTN_Maximize_Click(object sender, RoutedEventArgs e)
      {
         if (this.WindowState == WindowState.Maximized)
         {
            this.WindowState = WindowState.Normal;

            // Erstelle ein Image-Element
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/maximize.png")); // Pfad zu deinem Bild
            img.Width = 20; // Breite des Bildes
            img.Height = 20; // Höhe des Bildes

            this.BTN_Maximize.ToolTip = "Vergrößern";
            this.BTN_Maximize.Content = img;
         }
         else
         {
            this.WindowState = WindowState.Maximized;

            // Erstelle ein Image-Element
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/close_fullscreen.png")); // Pfad zu deinem Bild
            img.Width = 20; // Breite des Bildes
            img.Height = 20; // Höhe des Bildes

            this.BTN_Maximize.ToolTip = "Verkleinern";
            this.BTN_Maximize.Content = img;
         }
      }

      private void BTN_Close_Click(object sender, RoutedEventArgs e)
      {
         Application.Current.Shutdown();
      }

      private void BTN_Minimize_MouseEnter(object sender, MouseEventArgs e)
      {
         BTN_Minimize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005746"));
      }

      private void BTN_Minimize_MouseLeave(object sender, MouseEventArgs e)
      {
         BTN_Minimize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
      }
   }
}
