using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private PageModel _pmodel;
        public Main()
        {
            InitializeComponent();


            _pmodel = FindResource("pmodel") as PageModel ?? new();

            _pmodel.Navigate(new MainWindow());

        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (sizeInfo.HeightChanged)
                this.Top += (sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height) / 2;

            if (sizeInfo.WidthChanged)
                this.Left += (sizeInfo.PreviousSize.Width - sizeInfo.NewSize.Width) / 2;

            if (this.SizeToContent == SizeToContent.WidthAndHeight)
            {
                return;
            }
            Page page = this.Content as Page;
            if (page != null)
            {
                page.Width = this.ActualWidth - 20;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            Page page = this.Content as Page;
            if (page != null)
            {
                page.Width = this.ActualWidth;
                page.Height = this.ActualHeight;
            }
        }
    }
}
