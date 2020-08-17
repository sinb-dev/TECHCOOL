using System;
using System.Collections.Generic;

namespace TECHCOOL
{
    /// <summary>class <c>Result</c> represents a table of records. 
    /// Each row in the result contains a map (dictionary) of column/values</summary>
    public class Result : List<Dictionary<string,string>> 
    {

    }
    /// <summary>interface <c>ISQLetConnection</c> represents a connection to be used in SQLet. 
    /// Add custom connections to SQLet by creating classes that implements this interface
    /// and register them with <c>AddConnection</c></summary>
    public interface ISQLetConnection 
    {
        void Connect(string database, string host="localhost", string user=null, string password=null);
        /// <summary> method <c>Execute</c> runs a query statement on the database returns and integer. 
        /// Use for non-query statements like INSERT, DELETE, UPDATE, ALTER etc.</summary>
        int Execute(string sql);
        /// <summary> method <c>GetResult</c> runs a query statement on the databas and returns a <c>Result</c>. 
        /// Use for query statements like SELECT etc.</summary>
        Result GetResult(string sql);
        /// <summary> method <c>GetResult</c> runs a query statement on the databas 
        //// and returns a two-dimensional string array of rows and colums. 
        /// Use for query statements like SELECT etc.</summary>
        string[][] GetArray(string sql);
    }
    /// <summary>class <c>SQLet</c> can open and execute queries on a database. 
    /// Connect to SQLite, Sql Server or add a custom database connection with 
    //// <c>AddConnection(string name, ISQLetConnection instance)</c> </summary>
    public class SQLet 
    {
        private static ISQLetConnection instance;
        private static Dictionary<string,ISQLetConnection> connections = new Dictionary<string, ISQLetConnection>() {
            {"MSSQL", new SQLet_MSSQL()},
            {"SQLite", new SQLet_Sqlite()}
        };
        /// <summary> method <c>ConnectSQLite</c> opens a connection to a specified file. 
        /// The file created relative to the program folder</summary>
        public static void ConnectSQLite(string database_file) 
        {
            instance = connections["SQLite"];
            instance.Connect(database_file);
        }
        /// <summary> method <c>ConnectSqlServer</c> opens a connection to a specified Sql Server database. 
        /// Specify user and password</summary>
        public static void ConnectSqlServer(string database, string server, string user, string password) 
        {
            instance = connections["MSSQL"];
            instance.Connect(database, server, user, password);
        }
        /// <summary> method <c>ConnectSqlServer</c> opens a connection to a specified Sql Server database. 
        /// Uses windows logon user with TrustedConnection</summary>
        public static void ConnectSqlServer(string database, string server) 
        {
            instance = connections["MSSQL"];
            instance.Connect(database, server);
        }
        /// <summary> method <c>AddConnection</c> adds a connection instance to the library allowing
        /// for SQLet to send queries to that instance throw <c>Execute, GetResult, GetArray</c> </summary>
        public static void AddConnection(string name, ISQLetConnection instance) 
        {
            connections[name] = instance;
        }

        /// <summary> method <c>ConnectCustom</c> invokes the <c>Connect</c> 
        /// method on the custom database instance added with <c>AddConnection</c> </summary>
        public static void ConnectCustom(string name, string database, string server, string user, string password) 
        {
            if (connections.ContainsKey(name) == false) {
                throw new Exception("Cannot connect to custom database - "+name+" is not a registered connection. (Use AddConnection to register it)");
            }
            instance = connections[name];
            instance.Connect(database,server,user,password);
        }
        /// <summary> method <c>GetArray</c> invokes <c>ISQLetConnection.GetResult</c> on the current instance and returns a result object </summary>
        public static Result GetResult(string sql) 
        {
            return instance.GetResult(sql);
        }
        /// <summary> method <c>GetArray</c> invokes <c>ISQLetConnection.GetArray</c> on the current instance and returns a string array </summary>
        public static string[][] GetArray(string sql) 
        {
            return instance.GetArray(sql);
        }
        /// <summary> method <c>Execute</c> invokes <c>ISQLetConnection.Execute</c> on the current instance and returns an integer </summary>
        public static int Execute(string sql)
        {
            return instance.Execute(sql);
        }
    }
}
