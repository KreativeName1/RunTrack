using RunTrack.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class TopBar : UserControl
    {

        private PageModel? _pmodel;
        public TopBar()
        {
            InitializeComponent();
            this.DataContext = this;
            _pmodel = FindResource("pmodel") as PageModel ?? new();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _pmodel?.Navigate(new MainWindow());

        }

        private void Credits_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _pmodel?.Navigate(new Credits());
        }
    }
}
