using ControlzEx.Theming;
using MahApps.Metro.Controls;
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
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
