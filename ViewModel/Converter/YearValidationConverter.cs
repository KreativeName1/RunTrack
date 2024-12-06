using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    //Konvertiert Werte für die Jahr-Validierung in einer WPF-Anwendung
    class YearValidationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int year && year < 1900)
                return 1900;
            return value;
        }
    }

}
