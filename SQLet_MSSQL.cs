using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
namespace TECHCOOL 
{
    
    class SQLet_MSSQL : ISQLetConnection
    {
        private string connectionString = "";
        
        private static SQLet_MSSQL instance = new SQLet_MSSQL();
        
        public void Connect(string database, string host, string user, string password) 
        {
            
            if (user != null && password == null || user == null && password != null) {
                throw new System.Exception("Missing user or password is null");
            }
            if (user == null) 
            {
                connectionString = $"Server={host};Database={database};Trusted_Connection=True;";
            }
            else 
            {
                connectionString = $"Server={host};Database={database};User Id={user};Password={password}";
            }
        }
        
        public int Execute(string sql) 
        {
            using (var connection = new SqlConnection(connectionString))
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
            using (var connection = new SqlConnection(connectionString))
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
                                record.Add(string.Format("{0}",reader.GetValue(i)));
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
            using (var connection = new SqlConnection(connectionString))
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
                            
                            /*switch (reader) {
                                case 
                            }*/
                            
                            record[reader.GetName(i)] = reader.IsDBNull(i) ? "NULL" : string.Format("{0}",reader.GetValue(i));
                        }
                        results.Add(record);
                    }
                    
                }
            }
            return results;
        }
    }
}