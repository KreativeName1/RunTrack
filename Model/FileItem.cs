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
      public string FileName { get; set; }
      public DateTime UploadDate { get; set; }
      public bool IsSelected { get; set; }

      public FileItem()
      {
      }

      public FileItem(string fileName, DateTime uploadDate)
      {
         this.FileName = fileName;
         this.UploadDate = uploadDate;
      }

      public static List<FileItem> AlleLesen()
      {
         return DBFile.AlleLesen();
      }
   }
}
