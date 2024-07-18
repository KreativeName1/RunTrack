using System.IO;

namespace Klimalauf
{
    internal class DBFile
    {
        public static List<FileItem> AlleLesen()
        {
            // get all file items from the directory "Dateien"
            List<FileItem> files = new List<FileItem>();
            Directory.CreateDirectory("Dateien");
            string[] filePaths = Directory.GetFiles("Dateien");
            foreach (string filePath in filePaths)
            {
                FileInfo fi = new FileInfo(filePath);
                if (fi.Extension == ".db-shm" || fi.Extension == ".db-wal") continue;
                files.Add(new FileItem(fi.Name, fi.CreationTime));
            }
            return files;
        }
    }
}
