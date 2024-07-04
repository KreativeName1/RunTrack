using Klimalauf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
    public class FileItem
    {
      public string fileName { get; set; }
      public DateTime uploadDate { get; set; }
      public bool IsSelected { get; set; }

      public FileItem()
      {
      }

      public FileItem(string fileName, DateTime uploadDate)
      {
         this.fileName = fileName;
         this.uploadDate = uploadDate;
      }

      public static List<FileItem> AlleLesen()
      {
         return DBFile.AlleLesen();
      }
   }
}
