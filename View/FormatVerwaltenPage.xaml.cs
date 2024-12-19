using FullControls.Controls;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für FormatVerwaltenPage.xaml
    /// </summary>
    public partial class FormatVerwaltenPage : Page
    {
        private MainModel? _model;
        private PDFEditorModel _pemodel;

        public FormatVerwaltenPage()
        {
            InitializeComponent();
            _model = FindResource("pmodel") as MainModel ?? new MainModel();
            _pemodel = new PDFEditorModel();
            _pemodel.LoadData();
            DataContext = _pemodel;

        }

        private void Loeschen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int formatId)
            {
                var format = _pemodel.Formate?.FirstOrDefault(f => f.Id == formatId);
                if (format != null)
                {

                    if (new Popup().Display("Bestätigung", $"Möchten Sie das Format '{format.Name.ToUpper()}' wirklich löschen?", PopupType.Warning, PopupButtons.YesNo) == true)
                    {
                        using (var db = new LaufDBContext())
                        {
                            db.Formate.Remove(format);
                            db.SaveChanges();
                        }
                        _pemodel.Formate?.Remove(format);
                    }
                }
            }
        }


        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            // find last page that is a PDFEditor
            _model?.Navigate(_model?.History.LastOrDefault(p => p is PDFEditor), false);
        }

        private void dgFormate_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (glassScrollViewer != null)
            {
                glassScrollViewer.ScrollToVerticalOffset(glassScrollViewer.VerticalOffset - e.Delta / 3);
                e.Handled = true;
            }
        }

        private void Bearbeiten_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int formatId)
            {
                var format = _pemodel.Formate?.FirstOrDefault(f => f.Id == formatId);
                if (format != null)
                {
                    _model?.Navigate(new FormatBearbeitenPage(format), true);
                }
            }
        }



    }
}
