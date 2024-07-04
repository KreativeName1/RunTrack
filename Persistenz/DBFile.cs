using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Klimalauf.Persistenz
{
   internal class DBFile
   {
      public static List<FileItem> AlleLesen()
      {
         List<FileItem> lstFile = new List<FileItem>();

         using (MySqlConnection con = DBZugriff.OpenDB())
         {
            string sql = $"SELECT * FROM Pommes;";
            MySqlDataReader rdr = DBZugriff.ExecuteReader(sql, DBZugriff.OpenDB());

            while (rdr.Read())
            {
               FileItem t = GetDataFromReader(rdr);
               lstFile.Add(t);
            }
            rdr.Close();
         }
         return lstFile;
      }
      private static FileItem GetDataFromReader(MySqlDataReader rdr)
      {
         FileItem t = new FileItem();
         t.fileName = rdr.GetString("Name");
         t.uploadDate = rdr.GetDateTime("Zeit");
         return t;
      }
   }
}
