using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace TECHCOOL 
{
    class SQLet_Sqlite : ISQLetConnection
    {
        private string connectionString = "";
        private static SQLet_Sqlite instance = new SQLet_Sqlite();
        public void Connect(string database, string host, string user, string password) 
        {
            connectionString = $"Data Source={database};";
        }
        public int Execute(string sql) 
        {
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = sql;
                return command.ExecuteNonQuery();
            }
        }

        public string[][] GetArray(string sql) 
        {
            List<string[]> results = null;
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = sql;
                
                using (var reader = command.ExecuteReader())
                {
                    results = new List<string[]>();
                    
                    int rows = 0;
                    while (reader.Read())
                    {
                        var record = new List<string>();
                        rows++;
                        for (var i = 0; i < reader.FieldCount; i++) {
                            if (reader.IsDBNull(i)) 
                            {
                                record.Add("NULL");
                            } 
                            else 
                            {
                                record.Add(reader.GetString(i));
                            }
                        }
                        results.Add(record.ToArray());
                    }
                    
                }
            }
            return results.ToArray();
        }
        public Result GetResult(string sql) 
        {
            Result results = null;
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = sql;
                
                using (var reader = command.ExecuteReader())
                {
                    results = new Result();
                    
                    int rows = 0;
                    while (reader.Read())
                    {
                        var record = new Dictionary<string,string>();
                        rows++;
                        for (var i = 0; i < reader.FieldCount; i++) {
                            record[reader.GetName(i)] = reader.IsDBNull(i) ? "NULL" : reader.GetString(i);
                        }
                        results.Add(record);
                    }
                    
                }
            }
            return results;
        }
    }
}