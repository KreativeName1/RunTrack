using Klimalauf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
   public class ScanItem
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Schule { get; set; }

      public ScanItem(int id, string name, string schule)
      {
         Id = id;
         Name = name;
         Schule = schule;
      }

      public ScanItem() { }

      public ScanItem(int id)
      {
         Id = id;
      }

      public static List<ScanItem> AlleLesen()
      {
            return null;
      }
   }

}
