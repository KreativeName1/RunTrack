using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    // Definiert einen Konverter, der ein Objekt in einen booleschen Wert umwandelt
    public class ObjectToBoolConverter : IValueConverter
    {
        // Wandelt ein Objekt in einen booleschen Wert um
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Gibt true zurück, wenn das Objekt nicht null ist, andernfalls false
            return value != null;
        }

        // Diese Methode wird nicht implementiert, da die Umwandlung zurück nicht benötigt wird
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
