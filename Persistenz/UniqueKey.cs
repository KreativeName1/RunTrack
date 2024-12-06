using System.IO;
using System.IO.IsolatedStorage;

namespace RunTrack
{
    public class UniqueKey
    {
        // Methode zum Abrufen des Schlüssels
        public static string GetKey()
        {
            string key = string.Empty;

            // Zugriff auf den isolierten Speicher des Benutzers und der Assembly
            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            // Überprüft, ob die Datei "Key.txt" existiert
            if (store.FileExists("Key.txt"))
            {
                // Liest den Schlüssel aus der Datei
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("Key.txt", FileMode.Open, store))
                using (StreamReader reader = new StreamReader(stream))
                {
                    key = reader.ReadLine();
                }
            }

            // Wenn der Schlüssel leer ist, wird ein neuer Schlüssel generiert und erneut abgerufen
            if (string.IsNullOrEmpty(key))
            {
                GenerateKey();
                return GetKey();
            }
            return key;
        }

        // Methode zum Generieren eines neuen Schlüssels
        public static void GenerateKey()
        {
            // Erzeugt einen eindeutigen Schlüssel basierend auf dem Maschinenname, dem aktuellen Datum und einer GUID
            string uniqueKey = $"{Environment.MachineName}_{DateTime.UtcNow:yyyyMMdd}_{new string(Guid.NewGuid().ToString("N").Take(15).ToArray())}";

            // Zugriff auf den isolierten Speicher des Benutzers und der Assembly
            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            // Schreibt den neuen Schlüssel in die Datei "Key.txt"
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("Key.txt", FileMode.Create, store))
            using (StreamWriter writer = new StreamWriter(stream)) writer.WriteLine(uniqueKey);
        }

        // Methode zum Löschen des Schlüssels
        public static void DeleteKey()
        {
            // Zugriff auf den isolierten Speicher des Benutzers und der Assembly
            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            // Überprüft, ob die Datei "Key.txt" existiert und löscht sie
            if (store.FileExists("Key.txt")) store.DeleteFile("Key.txt");
        }
    }
}
