using System.IO;

namespace RunTrack
{
    public class CSVReader
    {
        public static List<object> ReadToList(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();
            if (Path.GetExtension(path).ToLower() != ".csv") throw new FileLoadException();
            if (new FileInfo(path).Length == 0) throw new FileLoadException();

            List<object> list = new();

            string[] lines = File.ReadAllLines(path);
            List<List<string>> parts = new();
            foreach (string line in lines) parts.Add(line.Split(';').ToList());
            List<string> first = parts[0];

            if (first.Count != 5) throw new Exception("Die Datei hat nicht die richtige Anzahl an Spalten.");
            parts.RemoveAt(0);

            foreach (List<string> line in parts)
            {
                if (line.Count != first.Count) throw new Exception("Die Datei hat nicht die richtige Anzahl an Spalten.");
                list.Add(new { Spalte1 = line[0], Spalte2 = line[1], Spalte3 = line[2], Spalte4 = line[3], Spalte5 = line[4] });
            }

            return list;
        }
    }
}
