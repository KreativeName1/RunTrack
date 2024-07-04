using MySql.Data.MySqlClient;
using System;

namespace Klimalauf.Persistenz
{
   internal static class DBZugriff
   {
      public static MySqlConnection OpenDB()
      {
         String constr = "Server=bszw.ddns.net;Uid= ;Pwd= ;database= ";

         MySqlConnection con = new MySqlConnection(constr);
         con.Open();

         return con;
      }

      public static void CloseDB(MySqlConnection con)
      {
         con.Close();
      }

      /*
      /// <summary>
      /// Hilfsmethode für alle INSERT, UPDATE, DELETE etc. Befehle
      /// </summary>
      /// <param name="sql">Der SQL-Befehl</param>
      /// <returns>Anzahl der betroffenen Datensätze</returns>
      public static int ExecuteNonQuery(string sql)
      {
         using (MySqlConnection con = DBZugriff.OpenDB())
         {
            MySqlCommand cmd = new MySqlCommand(sql, con);
            int anz = cmd.ExecuteNonQuery();    // gibt die Anzahl der betroffenen Zeilen zurück
            return anz;
         }
      }
      */

      /// <summary>
      /// Hilfsmethode für alle INSERT, UPDATE, DELETE etc. Befehle
      /// </summary>
      /// <param name="sql">Der SQL-Befehl</param>
      /// <returns>Anzahl der betroffenen Datensätze</returns>
      public static int ExecuteNonQuery(string sql)
      {
         MySqlConnection con = DBZugriff.OpenDB();

         try
         {
            MySqlCommand cmd = new MySqlCommand(sql, con);
            int anz = cmd.ExecuteNonQuery();    // gibt die Anzahl der betroffenen Zeilen zurück
            DBZugriff.CloseDB(con);
            return anz;
         }
         catch (Exception) 
         {
            throw;      // die soeben gefangene Exception wird nach dem finally weiter geworfen
         }
         finally
         {
            DBZugriff.CloseDB(con);
         }

      }

      public static MySqlDataReader ExecuteReader(string sql, MySqlConnection con) 
      {
         MySqlCommand cmd = new MySqlCommand(sql, con);
         return cmd.ExecuteReader();
      }


      public static int GetLastInsertId(MySqlConnection con)
      {
         string sql = "SELECT LAST_INSET_ID()";

         MySqlCommand cmd = new MySqlCommand(sql, con);
         int id = Convert.ToInt32(cmd.ExecuteNonQuery());

         return id;
      }

   }
}
