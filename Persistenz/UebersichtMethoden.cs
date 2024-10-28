using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RunTrack
{
    public class UebersichtMethoden
    {
        public static int CurrentSelectedRow { get; set; } = -1;

        public static void SearchDataGrid(DataGrid dataGrid, string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            // Liste zurücksetzen
            foreach (var row in dataGrid.Items)
            {
                DataGridRow dataGridRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(row);
                if (dataGridRow != null)
                {
                    dataGridRow.Visibility = System.Windows.Visibility.Visible;
                }
            }

            // Wenn kein Suchbegriff vorhanden, dann abbrechen
            if (string.IsNullOrEmpty(searchTerm)) return;

            // Suche durchführen
            foreach (var row in dataGrid.Items)
            {
                DataGridRow dataGridRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(row);
                if (dataGridRow != null)
                {
                    foreach (var cell in dataGrid.Columns)
                    {
                        var cellContent = cell.GetCellContent(dataGridRow) as FrameworkElement;
                        if (cellContent is TextBlock textBlock)
                        {
                            if (textBlock.Text.ToLower().Trim().Contains(searchTerm))
                            {
                                dataGridRow.Visibility = Visibility.Visible;
                                break;
                            }
                            else dataGridRow.Visibility = Visibility.Collapsed;
                        }
                        else if (cellContent is ComboBox comboBox)
                        {
                            var comboBoxText = comboBox.Text.ToLower().Trim();
                            if (comboBoxText.Contains(searchTerm))
                            {
                                dataGridRow.Visibility = Visibility.Visible;
                                break;
                            }
                            else dataGridRow.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }


        public static void SelectSearchedRow(DataGrid dataGrid, bool down, string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            int startRow = down ? CurrentSelectedRow + 1 : CurrentSelectedRow - 1;
            if (startRow < 0) startRow = dataGrid.Items.Count - 1;
            if (startRow >= dataGrid.Items.Count) startRow = 0;

            bool found = false;
            for (int i = startRow; i < dataGrid.Items.Count && i >= 0; i += down ? 1 : -1)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                if (row == null) continue;

                foreach (var cell in dataGrid.Columns)
                {
                    TextBlock? cellContent = cell.GetCellContent(row) as TextBlock;
                    if (cellContent != null && cellContent.Text.ToLower().Contains(searchTerm))
                    {
                        dataGrid.SelectedItem = dataGrid.Items[i];
                        dataGrid.ScrollIntoView(dataGrid.Items[i]);
                        row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        CurrentSelectedRow = i;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }

            if (!found)
            {
                CurrentSelectedRow = -1;
                SelectSearchedRow(dataGrid, down, searchTerm);
            }
        }
    }
}
