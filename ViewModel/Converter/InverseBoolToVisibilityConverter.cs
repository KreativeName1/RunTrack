using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RunTrack
{
    // Diese Klasse implementiert IValueConverter und dient dazu, boolesche Werte in Sichtbarkeitswerte umzuwandeln
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        // Diese Methode konvertiert einen booleschen Wert in einen Sichtbarkeitswert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Wenn der boolesche Wert wahr ist, wird Visibility.Collapsed zurückgegeben, andernfalls Visibility.Visible
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        // Diese Methode wird nicht implementiert, da die Rückkonvertierung nicht benötigt wird
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
