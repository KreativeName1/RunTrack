using System.Data.Entity;
using System.Globalization;
using System.Windows.Data;

namespace RunTrack
{
    // Diese Klasse implementiert die IValueConverter-Schnittstelle, um die RundenArt eines Läufers zu konvertieren.
    public class RundenArtConverter : IValueConverter
    {
        // Diese Methode konvertiert ein Objekt in ein anderes.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Das übergebene Objekt wird in eine Runde umgewandelt.
            Runde runde = (Runde)value;
            // Der Läufer der Runde wird abgerufen.
            Laeufer? laeufer = runde.Laeufer;

            // Eine neue Datenbankverbindung wird geöffnet.
            using (LaufDBContext db = new())
            {
                // Die RundenArt des Läufers wird aus der Datenbank abgerufen.
                laeufer.RundenArt = db.RundenArten.FirstOrDefault(r => r.Id == laeufer.RundenArtId);
            }

            // Wenn die RundenArt des Läufers nicht gefunden wurde...
            if (laeufer.RundenArt == null)
            {
                // Der Läufer wird in einen Schüler umgewandelt.
                Schueler schueler = laeufer as Schueler;
                RundenArt rundenArt;
                // Eine neue Datenbankverbindung wird geöffnet.
                using (LaufDBContext db = new())
                {
                    // Die Klasse des Schülers wird aus der Datenbank abgerufen, einschließlich der RundenArt.
                    Klasse klasse = db.Klassen.Include(k => k.RundenArt).FirstOrDefault(k => k.Id == schueler.KlasseId);
                    // Die RundenArt der Klasse wird abgerufen.
                    rundenArt = db.RundenArten.FirstOrDefault(r => r.Id == klasse.RundenArtId);
                    rundenArt = klasse.RundenArt;
                }
                // Die RundenArt der Klasse wird zurückgegeben.
                return rundenArt;
            }
            else
            {
                // Wenn die RundenArt des Läufers gefunden wurde, wird sie zurückgegeben.
                return laeufer.RundenArt;
            }
        }

        // Diese Methode wird nicht implementiert.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
