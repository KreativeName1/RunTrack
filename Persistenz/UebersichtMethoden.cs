using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Klimalauf
{
    public class UebersichtMethoden
    {
        public static int CurrentSelectedRow { get; set; }

        public static void SearchDataGrid(DataGrid dataGrid, string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            foreach (var row in dataGrid.Items)
            {
                foreach (var cell in dataGrid.Columns)
                {
                    var cellContent = cell.GetCellContent(row);
                    if (cellContent is TextBlock textBlock)
                    {
                        if (textBlock.Text.ToLower().Contains(searchTerm) && !string.IsNullOrEmpty(searchTerm))
                        {
                            textBlock.Background = Brushes.OrangeRed;
                        }
                        else
                        {
                            textBlock.Background = Brushes.White;
                        }
                    }
                }
            }
        }

        public static void SelectSearchedRow(DataGrid dataGrid, bool down, string searchterm)
        {
            if (CurrentSelectedRow >= dataGrid.Items.Count - 1) CurrentSelectedRow = 0;
            searchterm = searchterm.ToLower();
            bool breakOut = false;
            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                if (row == null) continue;
                foreach (var cell in dataGrid.Columns)
                {
                    TextBlock cellContent = cell.GetCellContent(row) as TextBlock;
                    string cellText = cellContent.Text.ToLower();
                    if (cellContent != null && cellText.Contains(searchterm))
                    {
                        if (i <= CurrentSelectedRow) continue;
                        object item = dataGrid.Items[i];
                        CurrentSelectedRow = i;
                        dataGrid.SelectedItem = item;
                        dataGrid.ScrollIntoView(item);
                        row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        breakOut = true;
                        break;
                    }
                }
                if (breakOut) break;
            }
        }
    }
}
