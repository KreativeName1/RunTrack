using System.IO;
using System.Text;

namespace TracerFile
{
    // Definiert die verschiedenen Protokollierungsstufen
    public enum TraceLevel
    {
        None,
        Info,
        Warning,
        Error
    }
    public class Tracer
    {
        // Liste der verwendeten Logger-IDs
        public static List<string> UsedLoggerId = new List<string>();
        // Name der Protokolldatei
        public string FileName { get; set; }
        // Eindeutige ID des Loggers
        public string LoggerId { get; private set; }

        // Konstruktor, der den Dateinamen und eine eindeutige Logger-ID festlegt
        public Tracer(string fileName)
        {
            FileName = fileName;
            LoggerId = GenerateRandomId(10);
            while (UsedLoggerId.Contains(LoggerId)) LoggerId = GenerateRandomId(10);
            UsedLoggerId.Add(LoggerId);
        }

        // Generiert eine zufällige ID mit der angegebenen Länge
        public static string GenerateRandomId(int length)
        {
            string Characters = "#+-!?ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random Random = new Random();
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(Characters[Random.Next(Characters.Length)]);
            }
            return result.ToString();
        }

        // Protokolliert eine Nachricht mit der angegebenen Protokollierungsstufe
        public void Trace(string message, TraceLevel level = TraceLevel.None)
        {
            // DEBUG
            return;
            try
            {
                using (StreamWriter writer = File.AppendText(FileName))
                {
                    if (level == TraceLevel.None)
                        writer.WriteLine($"{DateTime.Now} [{LoggerId}] {message}");
                    else
                        writer.WriteLine($"{DateTime.Now} [{LoggerId}] - {level}: {message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Schreiben in die Datei {FileName}", ex);
            }
        }

        // Protokolliert eine Nachricht, wenn die Bedingung erfüllt ist
        public void TraceIf(bool condition, string message, TraceLevel level = TraceLevel.None)
        {
            if (condition)
            {
                Trace(message, level);
            }
        }
    }
}
