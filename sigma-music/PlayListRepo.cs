using Microsoft.Data.Sqlite;

namespace sigma_music;

public class PlayListRepo
{
    private bool _isReady = false;

    public PlayListRepo()
    {
        Console.WriteLine("Play lists init...");
        Db.GetVersion();
        if (!IsDbHaveData())
        {
            //try to create table if table not exist
            if (!CreateTables())
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
        if (Db.IsTableExists("PlayList") && Db.IsTableExists("SongsInPlayLists"))
        {
            //Console.WriteLine("Music table already exists, nothing to do");
            return true;
        } 
        Console.WriteLine("Music table does not exist");
        return false;
    }

    private bool CreateTables()
    {
        Console.WriteLine("Creating table...");
        const string sql = "create table PlayList\n(\n    Id   INTEGER\n        constraint PlayList_pk\n            primary key autoincrement,\n    Name TEXT\n        constraint PlayList_pk_2\n            unique\n);\n\ncreate index PlayList_Name_indexx\n    on PlayList (Name);\n\ncreate index PlayList_Name_index\n    on PlayList (Name); create table SongsInPlayLists\n(\n    SongId     INTEGER\n        constraint SongsInPlayLists_Music_Id_fk\n            references Music,\n    PlayListId INTEGER\n        constraint SongsInPlayLists_PlayList_Id_fk\n            references PlayList,\n    constraint SongsInPlayLists_pk\n        unique (SongId, PlayListId)\n);";
        Db.RunSqlNonQuery(sql);
        Console.WriteLine("Done!");
        return IsDbHaveData(); 
    }

    private bool DeleteTable()
    {
        Console.WriteLine("Deleting table...");
        const string sql = "PRAGMA foreign_keys = OFF; drop table if exists SongsInPlayLists; drop table if exists PlayList; PRAGMA foreign_keys = ON;";
        Db.RunSqlNonQuery(sql);
        Console.WriteLine("Done!");
        return !IsDbHaveData(); //invert false to true due to deleting the table
    }

    public void DebugToggle()
    {
        DeleteTable();
        CreateTables();
    }
    
    public bool IsGetReady() => _isReady;
    
    //Select and Search
    public Results GetPlayListByName(string name)
    {
        var result = new Results("GetPlayList", [name]);
        if (string.IsNullOrEmpty(name))
        {
            result.AddMessage("Received NULL or empty play list name");
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
        
        const string sql = "SELECT *\nFROM PlayList\nWHERE Name = @name";
        var parameters = new SqliteParameter("@name", name);
        var dbAnswer = Db.RunSqlReader(sql, [parameters]);
        //Try to parse DB answer to playList db object
        var playlist = MapObjectToPlayListDb(dbAnswer).ReturnValues;
        result.Messages.Add($"Search: {name} -> {playlist?.Count??0} founded");
        result.SetReturnValue(playlist??[]);
        result.SetResult(true);
        
        result.AddMessage("Returned true!");
        return result;
    }

    public Results CanIAddThisSongToPlayList(SongDb songsCall, PlayListDb playListCall, MusicRepo musicRepo)
    {
        var result = new Results("CanIAddThisSongToPlayList", [songsCall, playListCall]);
        if (string.IsNullOrEmpty(playListCall.Name))
        {
            result.AddMessage("Received NULL or empty call params");
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
        
        //Get songs from playList
        var songsDb = GetSongsFromPlayList(playListCall, musicRepo).ReturnValues?.OfType<SongDb>().ToArray()??[];
        // Get song from call param
        var newSongInPlayList = musicRepo.GetSongById(songsCall.Id).ReturnValues?.OfType<SongDb>().ToArray().FirstOrDefault();
        result.AddMessage(newSongInPlayList?.ToString()??"not found song");
        if (newSongInPlayList == null) return result;
        // Go thou songs from playlist to check if a new song fits requirements
        
        //if play list empty then we can add whatever we want
        if (songsDb.Length == 0)
        {
            result.AddMessage("No songs found in a playlist than we can add all songs");
            result.SetResult(true);
            return result;
        }
        //Check if it is allready in playlist
        foreach (var oldSong in songsDb)
        {
            if (oldSong.Id == newSongInPlayList.Id)
            {
                result.AddMessage("Song already exists");
                return result;
            }
        }
        
        //if any common tags
        var new_song_tags = SongDto.SongDb_to_Song(newSongInPlayList).Tags;
        foreach (var oldSong in songsDb)
        {
            var oldtags = SongDto.SongDb_to_Song(oldSong).Tags;
            if (new_song_tags.Intersect(oldtags).Any())
            {
                result.AddMessage("Common tags exist!");
                result.SetResult(true);
            }
        }
        
        //if any common genres
        var new_song_genres = SongDto.SongDb_to_Song(newSongInPlayList).Genres;
        foreach (var oldSong in songsDb)
        {
            var oldtags = SongDto.SongDb_to_Song(oldSong).Genres;
            if (new_song_genres.Intersect(oldtags).Any())
            {
                result.AddMessage("Common genres exist!");
                result.SetResult(false);
                return result;
            }
        }
        
        result.AddMessage("Can be added!");
        return result;
    }
    
    public Results GetSongsFromPlayList(PlayListDb playListCall, MusicRepo musicRepo)
    {
        var result = new Results("GetSongsFromPlayList", [playListCall]);
        if (string.IsNullOrEmpty(playListCall.Name))
        {
            result.AddMessage("Received NULL or empty call params");
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
        
        //Get songs id's from playList
        const string sql = "SELECT *\nFROM SongsInPlayLists\nWHERE PlayListId = @pid";
        var parameters = new SqliteParameter("@pid", playListCall.Id);
        
        var dbAnswer = Db.RunSqlReader(sql, [parameters]);
        //Try to parse DB answer to playList db object
        var songInPlayLists = MapObjectToSongInPlayListDb(dbAnswer).ReturnValues?.OfType<SongsInPlayListDb>().ToArray()??[];
        foreach (var song in songInPlayLists)
        {
            //Get songDB object from songInPlayList object
            var songId = song.SongId;
            var songResult = musicRepo.GetSongById(songId);
            var songData = songResult.ReturnValues?.OfType<SongDb>().FirstOrDefault();
            if (songData == null) continue;
            result.AddMessage($"Song ID: {songId} returned");
            result.AddReturnValue(songData);
            result.SetResult(true);
        }
        return result;
    }
    
    
    
    //CUR - Songs to playLists

    public Results InsertSongToPlayList(IEnumerable<SongsInPlayListDb> connections)
    {
        var callParam = connections as SongsInPlayListDb[] ?? connections.ToArray(); // Get an array to go thou
        var result = new Results("InsertSongToPlayList", callParam);
        const string sql = "INSERT INTO SongsInPlayLists (SongId, PlayListId)\nVALUES (@sid, @pid);";
        // Save sql result to nextResult
        result.SetChildrenResult(RepoCud(callParam, sql));
        result.AddMessage(sql);
        
        // Use result data from children element
        result.SetReturnValue(result.NextResult?.ReturnValues ?? [false]);
        result.SetResult(result.NextResult?.Result ?? false);
        return result;
    }
    
    public Results DeleteSongToPlayList(IEnumerable<SongsInPlayListDb> connections)
    {
        var callParam = connections as SongsInPlayListDb[] ?? connections.ToArray(); // Get an array to go thou
        var result = new Results("UpdateSongToPlayList", callParam);
        const string sql = "DELETE FROM SongsInPlayLists\nWHERE SongId = @sid\n AND PlayListId = @pid";
        // Save sql result to nextResult
        result.SetChildrenResult(RepoCud(callParam, sql));
        result.AddMessage(sql);
        
        // Use result data from children element
        result.SetReturnValue(result.NextResult?.ReturnValues ?? [false]);
        result.SetResult(result.NextResult?.Result ?? false);
        return result;
    }
    
    
    
    //CUR - PlayLists

    public Results InsertPlayList(IEnumerable<PlayListDb> list)
    {
        var callParam = list as PlayListDb[] ?? list.ToArray(); // Get an array to go thou
        var result = new Results("InsertPlayList", callParam);
        const string sql = "INSERT INTO PlayList (Name)\nVALUES (@name);\n";
        // Save sql result to nextResult
        result.SetChildrenResult(RepoCud(callParam, sql));
        result.AddMessage(sql);
        
        // Use result data from children element
        result.SetReturnValue(result.NextResult?.ReturnValues ?? [false]);
        result.SetResult(result.NextResult?.Result ?? false);
        return result;
    }
    
    public Results UpdatePlayList(IEnumerable<PlayListDb> list)
    {
        var callParam = list as PlayListDb[] ?? list.ToArray(); // Get an array to go thou
        var result = new Results("UpdatePlayList", callParam);
        const string sql = "UPDATE PlayList\nSET Name = @name\nWHERE Id = @id;\n";
        // Save sql result to nextResult
        result.SetChildrenResult(RepoCud(callParam, sql));
        result.AddMessage(sql);
        
        // Use result data from children element
        result.SetReturnValue(result.NextResult?.ReturnValues ?? [false]);
        result.SetResult(result.NextResult?.Result ?? false);
        return result;
    }
    
    public Results DeletePlayList(IEnumerable<PlayListDb> list)
    {
        var callParam = list as PlayListDb[] ?? list.ToArray(); // Get an array to go thou
        var result = new Results("DeletePlayList", callParam);
        const string sql = "PRAGMA foreign_keys = OFF; DELETE\nFROM PlayList\nWHERE Id = @id; PRAGMA foreign_keys = ON;";
        // Save sql result to nextResult
        result.SetChildrenResult(RepoCud(callParam, sql));
        result.AddMessage(sql);
        
        // Use result data from children element
        result.SetReturnValue(result.NextResult?.ReturnValues ?? [false]);
        result.SetResult(result.NextResult?.Result ?? false);
        return result;
    }

    //Create Update Delete API
    private Results RepoCud(IEnumerable<PlayListDb> list, string sql)
    {
        var callParam = list as PlayListDb[] ?? list.ToArray(); // Get an array to go thou
        var result = new Results("Play list Repo-C-U-D", callParam);
        if (callParam.Length == 0)
        {
            result.AddMessage("Empty play list -> returns false");
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
                new SqliteParameter("@name",callParam[i].Name)
            };
            // Run SQL query
            var countRows = Db.RunSqlNonQuery(sql, parameters);
            //Logg data
            result.AddMessage($"{callParam[i].Name}");
            result.AddReturnValue(countRows);
        }
        return result;
    }
    
    private Results RepoCud(IEnumerable<SongsInPlayListDb> songsPlayLists, string sql)
    {
        var callParam = songsPlayLists as SongsInPlayListDb[] ?? songsPlayLists.ToArray(); // Get an array to go thou
        var result = new Results("SongsInPlayListDb Repo-C-U-D", callParam);
        if (callParam.Length == 0)
        {
            result.AddMessage("Empty SongsPlayLists -> returns false");
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
                new SqliteParameter("@sid", callParam[i].SongId),
                new SqliteParameter("@pid",callParam[i].PlayListId)
            };
            // Run SQL query
            var countRows = Db.RunSqlNonQuery(sql, parameters);
            //Logg data
            result.AddMessage($"{callParam[i].SongId} - {callParam[i].PlayListId}");
            result.AddReturnValue(countRows);
        }
        return result;
    }

    public static Results MapObjectToPlayListDb(object? obj)
    {
        var result = new Results("MapObjectToPlayListDb", [obj]);
        if (obj == null)
        {
            result.AddMessage("Null object received");
            return result;
        }
        try
        {
            if (obj.GetType() == typeof(SqliteDataReader))
            {
                Console.WriteLine("MapObjectToPlayListDb use SqliteDataReader");
                var reader = (SqliteDataReader)obj;
                result.AddMessage("Trying to map object to song db");
                while (reader.Read())
                {
                    var playlistobject = new PlayListDb()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                    };
                    
                    result.AddMessage(playlistobject.ToString()??"n/a");
                    result.AddReturnValue(playlistobject);
                
                }
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
    
    
    public static Results MapObjectToSongInPlayListDb(object? obj)
    {
        var result = new Results("MapObjectToSongInPlayListDb", [obj]);
        if (obj == null)
        {
            result.AddMessage("Null object received");
            return result;
        }
        try
        {
            if (obj.GetType() == typeof(SqliteDataReader))
            {
                Console.WriteLine("MapObjectToSongInPlayListDb use SqliteDataReader");
                var reader = (SqliteDataReader)obj;
                result.AddMessage("Trying to map object to song db");
                while (reader.Read())
                {
                    var songsInPlayListDbObj = new SongsInPlayListDb()
                    {
                        SongId = reader.GetInt32(reader.GetOrdinal("SongId")),
                        PlayListId = reader.GetInt32(reader.GetOrdinal("PlayListId")),
                    };
                    
                    result.AddMessage(songsInPlayListDbObj.ToString()??"n/a");
                    result.AddReturnValue(songsInPlayListDbObj);
                
                }
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