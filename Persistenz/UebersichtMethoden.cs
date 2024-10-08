﻿using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RunTrack
{
    public class UebersichtMethoden
    {
        public static int CurrentSelectedRow { get; set; } = -1;

        /*public static void SetSelectedRow(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row != null)
            {
                CurrentSelectedRow = row.GetIndex();
            }
        }

        public static void Search(DataGrid dataGrid, TextBox searchBox)
        {
            string filter = searchBox.Text;
            if (filter == "")
            {
                dataGrid.Items.Filter = null;
            }
            else
            {
                foreach (var row in dataGrid.Items)
                {
                    foreach (var cell in dataGrid.Columns /* row.GetType().GetProperties() *//*)
                    {
                        if (row.ToString().Contains(filter))
                        {
                            dataGrid.Items.Filter = null;

                            // Highlight the row
                            cell.Foreground = Brushes.Red;
                            cell.CellStyle = DataGrid.CellStyleProperty;
                        }
                        else
                        {
                            dataGrid.Items.Filter = null;
                        }
                    }

                }
            }
        }

        public static void SearchDataGrid(DataGrid dataGrid, TextBox searchBox)
        {
            Search(dataGrid, searchBox);
        }

        */
        public static void SearchDataGrid(DataGrid dataGrid, string searchTerm)
        {
            List<int> lstFoundRows = new List<int>();
            searchTerm = searchTerm.ToLower();
            foreach (var row in dataGrid.Items)
            {
                DataGridRow dataGridRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(row);
                if (dataGridRow != null)
                {
                    foreach (var cell in dataGrid.Columns)
                    {
                        // int zeile = cell.DisplayIndex;
                        var cellContent = cell.GetCellContent(dataGridRow) as TextBlock;
                        if (cellContent != null)
                        {
                            if (cellContent.Text.ToLower().Contains(searchTerm) && !string.IsNullOrEmpty(searchTerm))
                            {
                                if (!lstFoundRows.Contains(dataGrid.Items.IndexOf(row)))
                                    lstFoundRows.Add(dataGrid.Items.IndexOf(row));
                                //lstFoundRows.Add(dataGrid.Items.IndexOf(row));
                                cellContent.Background = Brushes.OrangeRed;
                            }
                            else
                            {
                                cellContent.Background = Brushes.White;
                            }
                        }
                    }
                }
            }

            if (lstFoundRows.Count > 0)
            {
                dataGrid.ScrollIntoView(dataGrid.Items[0]);
                dataGrid.ScrollIntoView(dataGrid.Items[lstFoundRows[0]]);
            }
        }


        public static void SearchDataGrid_Old(DataGrid dataGrid, string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            foreach (var row in dataGrid.Items)
            {
                DataGridRow dataGridRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(row);
                if (dataGridRow != null)
                {
                    foreach (var cell in dataGrid.Columns)
                    {
                        var cellContent = cell.GetCellContent(dataGridRow) as TextBlock;
                        if (cellContent != null)
                        {
                            if (cellContent.Text.ToLower().Contains(searchTerm) && !string.IsNullOrEmpty(searchTerm))
                            {
                                cellContent.Background = Brushes.OrangeRed;
                            }
                            else
                            {
                                cellContent.Background = Brushes.White;
                            }
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
