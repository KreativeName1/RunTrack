using FullControls.Controls;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für FormatLoeschenPage.xaml
    /// </summary>
    public partial class FormatLoeschenPage : Page
    {
        private MainModel? _model;
        private PDFEditorModel _pemodel;

        public FormatLoeschenPage()
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

                    if (new Popup().Display($"Möchten Sie das Format '{format.Name.ToUpper()}' wirklich löschen?", "Bestätigung", PopupType.Warning, PopupButtons.YesNo) == true)
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
            _model?.Navigate(_model.History[^1], false);
        }

        private void dgFormate_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (glassScrollViewer != null)
            {
                glassScrollViewer.ScrollToVerticalOffset(glassScrollViewer.VerticalOffset - e.Delta / 3);
                e.Handled = true;
            }
        }
    }
}
