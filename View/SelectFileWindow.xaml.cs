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
        // Ausgewählte Datei
        public string SelectedFile { get; private set; }
        // Liste der Dateielemente
        private List<FileListItem> _fileItems;
        // Sortierrichtung für den Namen
        private bool _sortAscendingName = true;
        // Sortierrichtung für die Info
        private bool _sortAscendingInfo = true;

        // Konstruktor, der die Dateielemente initialisiert und sortiert
        public SelectFileWindow(List<FileListItem> fileItems)
        {
            InitializeComponent();
            _fileItems = fileItems;
            fileItems = fileItems.OrderBy(f => f.Name).ToList();
            UpdateListBox();
        }

        // Event-Handler für den OK-Button
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

        // Event-Handler für den Abbrechen-Button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        // Event-Handler für den Schließen-Button
        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Event-Handler für MouseEnter auf dem Schließen-Button
        private void BTN_Close_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B42041"));
        }

        // Event-Handler für MouseLeave auf dem Schließen-Button
        private void BTN_Close_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        }

        // Event-Handler für MouseDown auf der Info-Spalte
        private void tbInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _fileItems = _sortAscendingInfo
                ? _fileItems.OrderBy(f => f.Tooltip).ToList()
                : _fileItems.OrderByDescending(f => f.Tooltip).ToList();
            _sortAscendingInfo = !_sortAscendingInfo;
            UpdateListBox();
        }

        // Aktualisiert die ListBox mit den Dateielementen
        private void UpdateListBox()
        {
            FileListBox.ItemsSource = _fileItems;
        }

        // Event-Handler für MouseDown auf der Name-Spalte
        private void tbName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _fileItems = _sortAscendingName
                ? _fileItems.OrderBy(f => f.Name).ToList()
                : _fileItems.OrderByDescending(f => f.Name).ToList();
            _sortAscendingName = !_sortAscendingName;
            UpdateListBox();
        }

        // Event-Handler für MouseDown auf der Pfad-Spalte (auskommentiert)
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
