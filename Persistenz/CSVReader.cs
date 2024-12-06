using System.IO;

namespace RunTrack
{
    public class CSVReader
    {
        // Methode zum Lesen einer CSV-Datei und Rückgabe als Liste von Objekten
        public static List<object> ReadToList(string path)
        {
            // Überprüft, ob die Datei existiert, andernfalls wird eine FileNotFoundException ausgelöst
            if (!File.Exists(path)) throw new FileNotFoundException();
            // Überprüft, ob die Datei eine .csv-Datei ist, andernfalls wird eine FileLoadException ausgelöst
            if (Path.GetExtension(path).ToLower() != ".csv") throw new FileLoadException();
            // Überprüft, ob die Datei leer ist, andernfalls wird eine FileLoadException ausgelöst
            if (new FileInfo(path).Length == 0) throw new FileLoadException();

            // Initialisiert eine neue Liste von Objekten
            List<object> list = new();

            // Liest alle Zeilen der Datei
            string[] lines = File.ReadAllLines(path);
            // Initialisiert eine Liste von Listen von Zeichenketten
            List<List<string>> parts = new();
            // Teilt jede Zeile anhand des Trennzeichens ';' und fügt sie der parts-Liste hinzu
            foreach (string line in lines) parts.Add(line.Split(';').ToList());
            // Speichert die erste Zeile (Header) in einer separaten Liste
            List<string> first = parts[0];

            // Überprüft, ob die erste Zeile genau 5 Spalten hat, andernfalls wird eine Exception ausgelöst
            if (first.Count != 5) throw new Exception("Die Datei hat nicht die richtige Anzahl an Spalten.");
            // Entfernt die erste Zeile (Header) aus der parts-Liste
            parts.RemoveAt(0);

            // Iteriert über jede Zeile in der parts-Liste
            foreach (List<string> line in parts)
            {
                // Überprüft, ob jede Zeile die gleiche Anzahl an Spalten wie die Header-Zeile hat, andernfalls wird eine Exception ausgelöst
                if (line.Count != first.Count) throw new Exception("Die Datei hat nicht die richtige Anzahl an Spalten.");
                // Fügt ein neues anonymes Objekt mit den Spaltenwerten zur Liste hinzu
                list.Add(new { Spalte1 = line[0], Spalte2 = line[1], Spalte3 = line[2], Spalte4 = line[3], Spalte5 = line[4] });
            }

            // Gibt die Liste von Objekten zurück
            return list;
        }
    }
}
