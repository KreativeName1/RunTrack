using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    public class UebersichtMethoden
    {
        public static int CurrentSelectedRow { get; set; } = -1;

        // searchterm wird nicht mehr benötigt
        public static void SelectSearchedRow(DataGrid dataGrid, bool down)
        {
            // Momentan ausgewählte Zeile
            int startRow = down ? CurrentSelectedRow + 1 : CurrentSelectedRow - 1;

            // Wenn die Startzeile außerhalb des Bereichs liegt, dann setze sie auf das gegenüberliegende Ende
            if (startRow < 0) startRow = dataGrid.Items.Count - 1;
            if (startRow >= dataGrid.Items.Count) startRow = 0;

            // In der Liste nach oben oder unten gehen

            if (down)
            {
                dataGrid.SelectedIndex = startRow;
                dataGrid.ScrollIntoView(dataGrid.Items[startRow]);
                CurrentSelectedRow = startRow;

            }
            else
            {
                dataGrid.SelectedIndex = startRow;
                dataGrid.ScrollIntoView(dataGrid.Items[startRow]);
                CurrentSelectedRow = startRow;
            }
        }
    }
}
