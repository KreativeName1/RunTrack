using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    // Definiere die Klasse TimestampConverter, die das IValueConverter-Interface implementiert
    public class TimestampConverter : IValueConverter
    {
        // Implementiere die Convert-Methode, um den Zeitstempel in das gewünschte Format zu konvertieren
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Überprüfe, ob der Wert ein DateTime-Objekt ist
            if (value is DateTime timestamp)
            {
                // Konvertiere den Zeitstempel in das Format "dd.MM.yyyy, HH:mm:ss"
                return timestamp.ToString("dd.MM.yyyy" + ",\t" + "HH:mm:ss");
            }
            // Wenn der Wert kein DateTime-Objekt ist, gib den ursprünglichen Wert zurück
            return value;
        }

        // Implementiere die ConvertBack-Methode, die eine NotImplementedException auslöst
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
