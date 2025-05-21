namespace sigma_music;
using Microsoft.Data.Sqlite;

public static class Db
{
    private const string ConnectionString = "Data Source=db.sqlite";
    private static SqliteConnection _connection;
    private static SqliteCommand _command;
    static Db()
    {
        Console.WriteLine("Kek");
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();
        _command = _connection.CreateCommand();
    }

    public static void Get_Version()
    {
        Console.WriteLine(_connection.ServerVersion);
    }
}