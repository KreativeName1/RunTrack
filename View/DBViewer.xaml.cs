using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaction logic for DBViewer.xaml
    /// </summary>
    public partial class DBViewer : Page
    {
        private DBViewerModel _model { get; set; }
        public DBViewer(string path)
        {
            InitializeComponent();

            _model = FindResource("viewModel") as DBViewerModel;
            _model.Initialize(path);
        }


    }
}
