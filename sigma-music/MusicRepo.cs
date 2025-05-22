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
            //Console.WriteLine("Music table already exists, nothing to do");
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
        Console.WriteLine("Done!");
        return IsDbHaveData(); 
    }

    private bool DeleteTable()
    {
        Console.WriteLine("Deleting table...");
        const string sql = "drop table if exists Music;";
        Db.RunSqlNonQuery(sql);
        Console.WriteLine("Done!");
        return !IsDbHaveData(); //invert false to true due to deleting the table
    }

    public void DebugToggle()
    {
        DeleteTable();
        CreateTable();
    }
    
    public bool IsGetReady() => _isReady;
    
    //Select and Search
    public Results FindSongs(IEnumerable<string?> searchPhares)
    {
        var searchArray = searchPhares as string[] ?? searchPhares.ToArray();
        var result = new Results("FindSongs", searchArray);
        if (searchArray.Length == 0)
        {
            result.AddMessage("Received NULL array");
            result.SetResult(false);
            result.SetReturnValue([false]);
            return result;
        }
        if (!IsGetReady() || !IsDbHaveData())
        {
            result.AddMessage("IsGetReady() or IsDbHaveData() returns false");
            result.SetResult(false);
            result.SetReturnValue([false]);
            return result;
        } //Secure check
        
        const string sql = "SELECT *\nFROM Music\nWHERE Description LIKE CONCAT('%', @search, '%')\n   OR Tags LIKE CONCAT('%', @search, '%')\n   OR Genres LIKE CONCAT('%', @search, '%')\nORDER BY\n    CASE\n        WHEN Description LIKE CONCAT('%', @search, '%') THEN 1\n        WHEN Tags LIKE CONCAT('%', @search, '%') THEN 2\n        WHEN Genres LIKE CONCAT('%', @search, '%') THEN 3\n        ELSE 4\n        END;";
        for (var i = 0; i < searchArray.Length; i++)
        {
            var parameters = new SqliteParameter("@search", "%"+searchArray[i]+"%");
            var dbAnswer = Db.RunSqlReader(sql, [parameters]);
            //Try to parse DB answer to SongDB object
            var songs = MapObjectToSongDb(dbAnswer).ReturnValues;
            result.Messages.Add($"Search #{i + 1}: {searchArray[i]} -> {songs?.Count??0} founded");
            result.AddReturnValue(songs??[]);
        }
        Console.WriteLine("Done!");
        if (!(result.ReturnValues?.Count > 0)) return result;
        result.SetResult(true);
        result.AddMessage("Returned true!");
        return result;

    }

    public Results InsertSongs(IEnumerable<SongDb> songs)
    {
        var callParam = songs as SongDb[] ?? songs.ToArray(); // Get an array to go thou
        var result = new Results("InsertSongs", callParam);
        const string sql = "INSERT INTO Music (Name, ArtistId, AlbumId, Genres, Description, Tags)\nVALUES (@name, @artist, @album, @genres, @des, @tags);";
        // Save sql result to nextResult
        result.SetChildrenResult(RepoCud(callParam, sql));
        result.AddMessage(sql);
        
        // Use result data from children element
        result.SetReturnValue(result.NextResult?.ReturnValues ?? [false]);
        result.SetResult(result.NextResult?.Result ?? false);
        return result;
    }
    
    public Results UpdateSongs(IEnumerable<SongDb> songs)
    {
        var callParam = songs as SongDb[] ?? songs.ToArray(); // Get an array to go thou
        var result = new Results("UpdateSongs", callParam);
        const string sql = "UPDATE Music\nSET Id          = @id,\n    Name        = @name,\n    ArtistId    = @artist,\n    AlbumId     = @album,\n    Genres      = @genres,\n    Description = @des,\n    Tags        = @tags \nWHERE Id = @id;\n";
        // Save sql result to nextResult
        result.SetChildrenResult(RepoCud(callParam, sql));
        result.AddMessage(sql);
        
        // Use result data from children element
        result.SetReturnValue(result.NextResult?.ReturnValues ?? [false]);
        result.SetResult(result.NextResult?.Result ?? false);
        return result;
    }
    
    public Results DeleteSongs(IEnumerable<SongDb> songs)
    {
        var callParam = songs as SongDb[] ?? songs.ToArray(); // Get an array to go thou
        var result = new Results("DeleteSongs", callParam);
        const string sql = "DELETE \n FROM Music \n WHERE Id = @id;";
        // Save sql result to nextResult
        result.SetChildrenResult(RepoCud(callParam, sql));
        result.AddMessage(sql);
        
        // Use result data from children element
        result.SetReturnValue(result.NextResult?.ReturnValues ?? [false]);
        result.SetResult(result.NextResult?.Result ?? false);
        return result;
    }

    //Create Update Delete API
    private Results RepoCud(IEnumerable<SongDb> songs, string sql)
    {
        var callParam = songs as SongDb[] ?? songs.ToArray(); // Get an array to go thou
        var result = new Results("Repo-C-U-D", callParam);
        if (callParam.Length == 0)
        {
            result.AddMessage("Empty songs -> returns false");
            result.SetResult(false);
            result.SetReturnValue([false]);
            return result;
        }
        if (!IsGetReady() || !IsDbHaveData())
        {
            result.AddMessage("IsGetReady() or IsDbHaveData() returns false");
            result.SetResult(false);
            result.SetReturnValue([false]);
            return result;
        } //Secure check
        result.SetResult(true);
        result.AddMessage(sql);
        for (var i = 0; i < callParam.Length; i++)
        {
            // Create params - sql takes only that sql needs
            var parameters = new SqliteParameter[]
            {
                new SqliteParameter("@id", callParam[i].Id),
                new SqliteParameter("@name",callParam[i].Name),
                new SqliteParameter("@artist",callParam[i].ArtistId),
                new SqliteParameter("@album",callParam[i].AlbumId),
                new SqliteParameter("@genres",callParam[i].Genres),
                new SqliteParameter("@des",callParam[i].Description),
                new SqliteParameter("@tags",callParam[i].Tags)
            };
            // Run SQL query
            var countRows = Db.RunSqlNonQuery(sql, parameters);
            //Logg data
            result.AddMessage($"{callParam[i].Name}");
            result.AddReturnValue(countRows);
        }
        return result;
    }

    public static Results MapObjectToSongDb(object? obj)
    {
        var result = new Results("MapObjectToSongDb", [obj]);
        if (obj == null)
        {
            result.AddMessage("Null object received");
            return result;
        }
        try
        {
            if (obj.GetType() == typeof(SqliteDataReader))
            {
                Console.WriteLine("MapObjectToSongDb use SqliteDataReader");
                var reader = (SqliteDataReader)obj;
                result.AddMessage("Trying to map object to song db");
                while (reader.Read())
                {
                    var song = new SongDb()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        ArtistId = reader.GetInt32(reader.GetOrdinal("ArtistId")),
                        AlbumId = reader.GetInt32(reader.GetOrdinal("AlbumId")),
                        Genres = reader.GetString(reader.GetOrdinal("Genres")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        Tags = reader.GetString(reader.GetOrdinal("Tags"))
                    };
                    
                    result.AddMessage(song.ToString());
                    result.AddReturnValue(song);
                
                }
            }
            else
            {
                //Experimental API, not in use but should work =)
                var test = (SongDb)obj;
                result.AddMessage(test.ToString());
                result.AddReturnValue(test);
            }

            if (result.ReturnValues?.Count > 0)
            {
                result.SetResult(true);
            }
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result.AddMessage(e.ToString());
            result.AddMessage("Stack trace: "+e.StackTrace??"no stack trace");
            result.SetResult(false);
            result.SetReturnValue([false]);
            return result;
        }
    }
}