using System.Windows.Controls;

namespace RunTrack
{
    public class UebersichtMethoden
    {
        public static int CurrentSelectedRow { get; set; } = -1;

        // Methode zum Auswählen einer Zeile in einem DataGrid
        public static void SelectSearchedRow(DataGrid dataGrid, bool down)
        {
            // Startzeile basierend auf der aktuellen Auswahl und der Richtung (hoch oder runter)
            int startRow = down ? CurrentSelectedRow + 1 : CurrentSelectedRow - 1;

            // Wenn die Startzeile außerhalb des Bereichs liegt, dann setze sie auf das gegenüberliegende Ende
            if (startRow < 0) startRow = dataGrid.Items.Count - 1;
            if (startRow >= dataGrid.Items.Count) startRow = 0;

            // Schleife, um die Zeile im DataGrid auszuwählen und in den sichtbaren Bereich zu scrollen
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
