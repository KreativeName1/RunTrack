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
            //UniqueKey.DeleteKey();
            _pmodel = FindResource("pmodel") as MainModel ?? new();
            MainWindow main = new();
            _pmodel.CurrentPage = main;

            this.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.F11) ChangeState();
            };

            this.SizeChanged += MetroWindow_SizeChanged;

            BTN_Minimize.IsTabStop = false;
            BTN_Maximize.IsTabStop = false;
            BTN_Close.IsTabStop = false;
            ContentFrame.IsTabStop = false;
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

        private void ChangeSize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/maximize.png"));
                img.Width = 20;
                img.Height = 20;

                this.BTN_Maximize.ToolTip = "Vergrößern";
                this.BTN_Maximize.Content = img;
            }
            else
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/close_fullscreen.png"));
                img.Width = 15;
                img.Height = 15;

                this.BTN_Maximize.ToolTip = "Verkleinern";
                this.BTN_Maximize.Content = img;
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

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/maximize.png"));
                img.Width = 20;
                img.Height = 20;

                this.BTN_Maximize.ToolTip = "Vergrößern";
                this.BTN_Maximize.Content = img;
            }
            else
            {
                this.WindowState = WindowState.Maximized;

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/close_fullscreen.png"));
                img.Width = 15;
                img.Height = 15;

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

        private void BTN_Maximize_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Maximize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005746"));
        }
        private void BTN_Maximize_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Maximize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        }

        private void BTN_Close_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B42041"));
        }
        private void BTN_Close_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        }

        //private void BTN_Settings_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    BTN_Settings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005746"));
        //}

        //private void BTN_Settings_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    BTN_Settings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        //}


        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                //this.WindowState = WindowState.Normal;

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/maximize.png"));
                img.Width = 20;
                img.Height = 20;

                this.BTN_Maximize.ToolTip = "Vergrößern";
                this.BTN_Maximize.Content = img;
            }
            else
            {
                //this.WindowState = WindowState.Maximized;

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/close_fullscreen.png"));
                img.Width = 15;
                img.Height = 15;

                this.BTN_Maximize.ToolTip = "Verkleinern";
                this.BTN_Maximize.Content = img;
            }
        }
    }
}
