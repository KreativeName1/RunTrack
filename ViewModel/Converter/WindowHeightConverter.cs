using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    // Diese Klasse implementiert die IValueConverter-Schnittstelle, um die Höhe eines Fensters zu konvertieren
    public class WindowHeightConverter : IValueConverter
    {
        // Diese Methode konvertiert den Eingabewert in die Höhe des Fensters
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Überprüfe, ob der Wert eine Höhe darstellt
            if (value is double height)
            {
                return height; // Gibt die Höhe zurück, wenn der Wert ein double ist
            }
            return 0; // Gibt 0 zurück, wenn der Wert keine Höhe ist
        }

        // Diese Methode wird nicht implementiert und löst eine Ausnahme aus
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
