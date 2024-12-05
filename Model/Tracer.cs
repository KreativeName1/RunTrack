using System.IO;
using System.Text;

namespace TracerFile
{
    public enum TraceLevel
    {
        None,
        Info,
        Warning,
        Error
    }
    public class Tracer
    {
        public static List<string> UsedLoggerId = new List<string>();
        public string FileName { get; set; }
        public string LoggerId { get; private set; }

        public Tracer(string fileName)
        {
            FileName = fileName;
            LoggerId = GenerateRandomId(10);
            while (UsedLoggerId.Contains(LoggerId)) LoggerId = GenerateRandomId(10);
            UsedLoggerId.Add(LoggerId);
        }

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
                throw new Exception($"Error writing to file {FileName}", ex);
            }
        }

        public void TraceIf(bool condition, string message, TraceLevel level = TraceLevel.None)
        {
            if (condition)
            {
                Trace(message, level);
            }
        }
    }
}
