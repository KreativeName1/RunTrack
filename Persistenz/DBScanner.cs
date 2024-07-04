using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.WellKnownTypes.Field.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Klimalauf
{
   internal class DBScanner
   {

      public static List<ScanItem> AlleLesen()
      {
         List<ScanItem> lstScanner = new List<ScanItem>();

         using (MySqlConnection con = DBZugriff.OpenDB())
         {
            string sql = $"SELECT * FROM Pommes;";
            MySqlDataReader rdr = DBZugriff.ExecuteReader(sql, DBZugriff.OpenDB());

            while (rdr.Read())
            {
               ScanItem t = GetDataFromReader(rdr);
               lstScanner.Add(t);
            }
            rdr.Close();
         }
         return lstScanner;
      }
      private static ScanItem GetDataFromReader(MySqlDataReader rdr)
      {
         ScanItem t = new ScanItem();
         t.Id = rdr.GetInt32("Id");
         t.Name = rdr.GetString("Name");
         t.Schule = rdr.GetString("Name");
         return t;
      }
   }
}
