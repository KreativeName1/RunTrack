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

            BTN_Key.IsTabStop = false;
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



        

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                //this.WindowState = WindowState.Normal;

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/maximize.png"));
                img.Width = 20;
                img.Height = 20;

            }
            else
            {
                //this.WindowState = WindowState.Maximized;

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/titelbarIcons/close_fullscreen.png"));
                img.Width = 15;
                img.Height = 15;
            }
        }

        private void BTN_Key_Click(object sender, RoutedEventArgs e)
        {
            if (_pmodel?.CurrentPage is SystemKey) return;
            _pmodel?.Navigate(new SystemKey());
        }

        private void BTN_Key_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Key.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#58a394"));
        }

        private void BTN_Key_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Key.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007d64"));
        }

        private void BTN_Key_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BTN_Key.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078d7"));
        }
    }
}
