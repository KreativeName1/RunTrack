using Microsoft.Data.Sqlite;
using System.Data;

namespace RunTrack
{
    public class DBService
    {
        private string _connectionString;

        public DBService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<string> GetTableNames(string[]? blacklist = null)
        {
            var tables = new List<string>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT name FROM sqlite_master WHERE type='table';";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (blacklist == null || !blacklist.Contains(reader.GetString(0))) tables.Add(reader.GetString(0));
                    }
                }
            }
            return tables;
        }

        public DataTable GetTableData(string tableName)
        {
            var table = new DataTable();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var query = $"SELECT * FROM {tableName};";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader); // Liest die Daten in das DataTable-Objekt
                }
            }
            return table;
        }

    }
}
