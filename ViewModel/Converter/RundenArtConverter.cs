using System.Data.Entity;
using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    public class RundenArtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Runde runde = (Runde)value;
            Laeufer? laeufer = runde.Laeufer;

            using (LaufDBContext db = new())
            {
                laeufer.RundenArt = db.RundenArten.FirstOrDefault(r => r.Id == laeufer.RundenArtId);
            }

            if (laeufer.RundenArt == null)
            {
                Schueler schueler = laeufer as Schueler;
                RundenArt rundenArt;
                using (LaufDBContext db = new())
                {
                    Klasse klasse = db.Klassen.Include(k => k.RundenArt).FirstOrDefault(k => k.Id == schueler.KlasseId);
                    rundenArt = db.RundenArten.FirstOrDefault(r => r.Id == klasse.RundenArtId);
                    rundenArt = klasse.RundenArt;

                }
                return rundenArt;
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
