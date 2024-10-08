﻿using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    public class TimestampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime timestamp)
            {
                // Konvertiere den Zeitstempel in das gewünschte Format
                return timestamp.ToString("dd.MM.yyyy" + ",\t" + "HH:mm:ss");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
