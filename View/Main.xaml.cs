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
    public partial class Main : Window
    {
        private PageModel _pmodel;
        public Main()
        {
            InitializeComponent();

           
            _pmodel = FindResource("pmodel") as PageModel ?? new();
            MainWindow main = new();

            _pmodel.CurrentPage = main;
            _pmodel.PageTitle = main.Title;

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
                // set the page size to the window size
                page.Width = this.ActualWidth - 20;
            }
        }

        // on resize, change the page size
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }

        // on maximize and restore, change the page size
        private void Window_StateChanged(object sender, EventArgs e)
        {
            // get the page from the Window
            Page page = this.Content as Page;
            if (page != null)
            {
                // set the page size to the window size
                page.Width = this.ActualWidth;
                page.Height = this.ActualHeight;
            }
        }
    }
}
