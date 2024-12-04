using MahApps.Metro.Controls;
using RunTrack.Model;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für SelectFileWindow.xaml
    /// </summary>
    public partial class SelectFileWindow : MetroWindow
    {
        public string SelectedFile { get; private set; }
        private List<FileListItem> _fileItems;
        private bool _sortAscendingName = true;
        //private bool _sortAscendingPfad = true;
        private bool _sortAscendingInfo = true;

        public SelectFileWindow(List<FileListItem> fileItems)
        {
            InitializeComponent();
            //FileListBox.ItemsSource = fileItems;
            _fileItems = fileItems.OrderBy(f => f.Name).ToList();
            UpdateListBox();
        }


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileListBox.SelectedItem is FileListItem selectedItem)
            {
                SelectedFile = selectedItem.Pfad;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Datei aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BTN_Close_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B42041"));
        }
        private void BTN_Close_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        }

        private void tbInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _fileItems = _sortAscendingInfo
                ? _fileItems.OrderBy(f => f.Tooltip).ToList()
                : _fileItems.OrderByDescending(f => f.Tooltip).ToList();
            _sortAscendingInfo = !_sortAscendingInfo;
            UpdateListBox();
        }

        private void UpdateListBox()
        {
            FileListBox.ItemsSource = null;
            FileListBox.ItemsSource = _fileItems;
        }

        private void tbName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _fileItems = _sortAscendingName
                ? _fileItems.OrderBy(f => f.Name).ToList()
                : _fileItems.OrderByDescending(f => f.Name).ToList();
            _sortAscendingName = !_sortAscendingName;
            UpdateListBox();
        }

        //private void tbPfad_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    _fileItems = _sortAscendingPfad
        //        ? _fileItems.OrderBy(f => f.Pfad).ToList()
        //        : _fileItems.OrderByDescending(f => f.Pfad).ToList();
        //    _sortAscendingPfad = !_sortAscendingPfad;
        //    UpdateListBox();
        //}
    }
}
