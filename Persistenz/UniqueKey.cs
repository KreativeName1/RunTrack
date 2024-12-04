using System.IO;
using System.IO.IsolatedStorage;

namespace RunTrack
{
    public class UniqueKey
    {
        public static string GetKey()
        {
            string key = string.Empty;
            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (store.FileExists("Key.txt"))
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("Key.txt",
                 FileMode.Open, store))
                using (StreamReader reader = new StreamReader(stream))
                {
                    key = reader.ReadLine();
                }
            }

            if (string.IsNullOrEmpty(key))
            {
                GenerateKey();
                return GetKey();
            }
            return key;
        }

        public static void GenerateKey()
        {
            string uniqueKey = Environment.MachineName + "_" + Environment.UserName + "_" + Guid.NewGuid().ToString().Substring(0, 8);

            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("Key.txt", FileMode.Create, store))
            using (StreamWriter writer = new StreamWriter(stream)) writer.WriteLine(uniqueKey);
        }

        public static void DeleteKey()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (store.FileExists("Key.txt")) store.DeleteFile("Key.txt");
        }
    }
}
