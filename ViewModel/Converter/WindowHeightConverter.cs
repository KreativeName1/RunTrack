using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    public class WindowHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Überprüfe, ob der Wert eine Höhe darstellt
            if (value is double height)
            {
                return height;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
