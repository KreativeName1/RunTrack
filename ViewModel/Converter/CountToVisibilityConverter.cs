using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    // Converter-Klasse, die IValueConverter implementiert
    public class CountToVisibilityConverter : IValueConverter
    {
        // Methode zum Konvertieren eines Wertes
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Überprüfen, ob der Wert ein int ist und das Parameter ein string ist, der in ein int umgewandelt werden kann
            if (value is int count && parameter is string param && int.TryParse(param, out int threshold))
            {
                // Sichtbarkeit basierend auf dem Vergleich des Wertes mit dem Schwellenwert zurückgeben
                return count > threshold ? Visibility.Visible : Visibility.Collapsed;
            }
            // Standardmäßig Collapsed zurückgeben, wenn die Bedingungen nicht erfüllt sind
            return Visibility.Collapsed;
        }

        // Methode zum Zurückkonvertieren eines Wertes (nicht implementiert)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
