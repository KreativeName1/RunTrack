using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    // Interne Klasse, die das IValueConverter-Interface implementiert
    internal class NullToBoolConverter : IValueConverter
    {
        // Methode zum Konvertieren eines Wertes (von Objekt zu Bool)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Gibt true zurück, wenn der Wert nicht null ist, ansonsten false
            return value != null;
        }

        // Methode zum Zurückkonvertieren eines Wertes (nicht implementiert)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Gibt an, dass keine Rückkonvertierung durchgeführt wird
            return Binding.DoNothing;
        }
    }
}
