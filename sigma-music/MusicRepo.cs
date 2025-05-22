using Microsoft.Data.Sqlite;

namespace sigma_music;

public class MusicRepo
{
    private bool _isReady = false;

    public MusicRepo()
    {
        Console.WriteLine("MusicRepo init...");
        Db.GetVersion();
        if (!IsDbHaveData())
        {
            //try to create table if table not exist
            if (!CreateTable())
            {
                //failed to create table
                Console.WriteLine("Table creation failed");
                Console.WriteLine("Please restart application");
                Console.ReadLine();
            }
            else
            {
                //table is created!
                Console.WriteLine("Table creation succeeded!");
                Console.WriteLine("Application ready!");
                _isReady = true;
            }
        }
        else
        {
            //we have table so don't need to stress
            Console.WriteLine("Application ready!");
            _isReady = true;
        }
    }

    private bool IsDbHaveData()
    {
        if (Db.IsTableExists("Music"))
        {
            Console.WriteLine("Music table already exists, nothing to do");
            return true;
        } 
        Console.WriteLine("Music table does not exist");
        return false;
    }

    private bool CreateTable()
    {
        Console.WriteLine("Creating table...");
        const string sql = "create table Music\n(\n    Id          INTEGER\n        constraint Music_pk\n            primary key autoincrement,\n    Name        TEXT,\n    ArtistId    INTEGER,\n    AlbumId     INTEGER,\n    Genres      TEXT,\n    Description TEXT,\n    Tags        TEXT\n);\n\ncreate index Music_Name_index\n    on Music (Name);\n";
        Db.RunSqlNonQuery(sql);
        Console.WriteLine("Run checking...");
        return IsDbHaveData(); 
    }
    
    public bool IsGetReady() => _isReady;
    
    public bool 
    
}