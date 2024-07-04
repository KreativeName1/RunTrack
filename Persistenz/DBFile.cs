using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.IO;

namespace Klimalauf
{
   internal class DBFile
   {
      public static List<FileItem> AlleLesen()
      {
            // get all file items from the directory "Dateien"
            List<FileItem> files = new List<FileItem>();
            string[] filePaths = Directory.GetFiles("Dateien");
            foreach (string filePath in filePaths)
            {
                FileInfo fi = new FileInfo(filePath);
                files.Add(new FileItem(fi.Name, fi.CreationTime));
            }
            return files;
        }
   }
}
