using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RunTrack
{
    public class RundenArtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return "Keine Rundenart";
            }

            if (value is Schueler schueler)
            {
                return schueler.Klasse?.RundenArt;
            }
            else if (value is Laeufer laeufer)
            {
                return laeufer.RundenArt;
            }
            else
            {
                return "Ungültiger Datentyp";
            }
        }

        // Rückkonvertierung (nicht benötigt für ein-weg-Binding)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
