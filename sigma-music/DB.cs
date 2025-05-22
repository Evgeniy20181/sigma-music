using System.Data;
using SQLitePCL;

namespace sigma_music;
using Microsoft.Data.Sqlite;

public static class Db
{
    private const string ConnectionString = "Data Source=db.sqlite";
    private static SqliteConnection _connection;
    private static SqliteCommand _command;
    static Db()
    {
        Console.WriteLine("Db creating...");
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();
        _command = _connection.CreateCommand();
        Console.WriteLine("Db ready!");
    }
    
    public static SqliteDataReader? RunSqlReader(string sql, IEnumerable<SqliteParameter> parameters)
    {
        if(IsGetReady() != true) return null;
        try
        {
            Console.WriteLine("Executing SQL reader...");
            _command.CommandText = sql;
            _command.Parameters.AddRange(parameters);
            return _command.ExecuteReader();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    

    public static void RunSqlNonQuery(string sql)
    {
        if(IsGetReady() != true) return;
        _command.CommandText = sql;
        _command.ExecuteNonQuery();
    }
    
    public static int RunSqlNonQuery(string sql, IEnumerable<SqliteParameter> parameters)
    {
        if(IsGetReady() != true) return -2;
        _command.CommandText = sql;
        _command.Parameters.AddRange(parameters);
        return _command.ExecuteNonQuery();
    }
    

    public static bool IsTableExists(string tableName)
    {
        if(IsGetReady() != true) return false;
        _command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
        _command.Parameters.AddWithValue("@tableName", tableName);
        var result = _command.ExecuteScalar()?.ToString();
        if (result == null) return false;
        return result == tableName;
    }
    
    
    private static bool IsConnected()
    {
        return _connection.State == ConnectionState.Open;
    }

    private static bool IsGetReady()
    {
        if (IsConnected() != true) return false;
        _command = _connection.CreateCommand();
        return true;
    }

    public static void GetVersion()
    {
        Console.Write("Using SQLite DB: ");
        Console.WriteLine(_connection.ServerVersion);
        Console.WriteLine("Database:"+_connection.Database);
        Console.WriteLine(_connection.ConnectionString);
    }

    public static void Close()
    {
        _connection.Close();
    }
}