using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            Runde runde = (Runde)value;
            Laeufer? laeufer = runde.Laeufer;

            if (laeufer.RundenArt == null)
            {
                Schueler schueler = laeufer as Schueler;
                return schueler.Klasse.RundenArt;
            }
            else
            {
                return laeufer.RundenArt;  
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
