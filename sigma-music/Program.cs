using sigma_music;

//Debug songs
const int startDelay = 2000;
var songs = new List<SongDb>(10)
{
    new()
    {
        Id = 1, Name = "Bohemian Rhapsody", ArtistId = 1, AlbumId = 1, Genres = "Rock",
        Description = "Classic Queen hit", Tags = "classic,70s,rock"
    },
    new()
    {
        Id = 2, Name = "Billie Jean", ArtistId = 2, AlbumId = 2, Genres = "Pop-disco",
        Description = "Michael Jackson hit", Tags = "pop,80s,legend"
    },
    new()
    {
        Id = 3, Name = "Imagine", ArtistId = 3, AlbumId = 3, Genres = "Pop Rock",
        Description = "John Lennon song", Tags = "peace,classic,legend"
    },
    new()
    {
        Id = 4, Name = "Smells Like Teen Spirit", ArtistId = 4, AlbumId = 4, Genres = "Grunge",
        Description = "Nirvana anthem", Tags = "grunge,90s,alternative"
    },
    new()
    {
        Id = 5, Name = "Shape of You", ArtistId = 5, AlbumId = 5, Genres = "Pop",
        Description = "Ed Sheeran chart-topper", Tags = "pop,modern,dance"
    },
    new()
    {
        Id = 6, Name = "Lose Yourself", ArtistId = 6, AlbumId = 6, Genres = "Hip-Hop",
        Description = "Eminem’s Oscar-winning track", Tags = "rap,motivation,2000s"
    },
    new()
    {
        Id = 7, Name = "Rolling in the Deep", ArtistId = 7, AlbumId = 7, Genres = "Soul, Pop",
        Description = "Adele breakout song", Tags = "soul,pop,ballad"
    },
    new()
    {
        Id = 8, Name = "Hotel California", ArtistId = 8, AlbumId = 8, Genres = "Rock",
        Description = "The Eagles’ masterpiece", Tags = "rock,70s,classic"
    },
    new()
    {
        Id = 9, Name = "Stairway to Heaven", ArtistId = 9, AlbumId = 9, Genres = "Rock",
        Description = "Led Zeppelin’s epic", Tags = "rock,classic,epic"
    },
    new()
    {
        Id = 10, Name = "Blinding Lights", ArtistId = 10, AlbumId = 10, Genres = "Synthpop",
        Description = "The Weeknd’s hit", Tags = "synthwave,modern,pop"
    }
};


Console.WriteLine("Song orkiestry program");
Console.WriteLine(
    "\n__   __         _                _ _  __   __    _                  _         \n\\ \\ / /        | |              (_|_) \\ \\ / /   | |                | |        \n \\ V /_____   _| |__   ___ _ __  _ _   \\ V /__ _| |_ ___  ___ _ __ | | _____  \n  \\ // _ \\ \\ / / '_ \\ / _ \\ '_ \\| | |   \\ // _` | __/ __|/ _ \\ '_ \\| |/ / _ \\ \n  | |  __/\\ V /| | | |  __/ | | | | |   | | (_| | |_\\__ \\  __/ | | |   < (_) |\n  \\_/\\___| \\_/ |_| |_|\\___|_| |_|_|_|   \\_/\\__,_|\\__|___/\\___|_| |_|_|\\_\\___/ \n                                                                              \n                                                                              \n");
var updateSong = new SongDb()
{
    Id = 10, Name = "Random song", ArtistId = 1, AlbumId = 2, Genres = "Genres",
    Description = "Hit", Tags = "tags, tags, tags"
};


var musicRepo = new MusicRepo();
var playListRepo = new PlayListRepo();
playListRepo.DebugToggle();
musicRepo.DebugToggle();
musicRepo.InsertSongs(songs);
playListRepo.InsertPlayList([new PlayListDb() { Id = 10, Name = "PlayList1" }]);
playListRepo.InsertSongToPlayList([new SongsInPlayListDb(){PlayListId = 1, SongId = 5}]);
Thread.Sleep(startDelay);
Console.Clear();


//Menu

if (!musicRepo.IsGetReady())
{
    Console.WriteLine("Repository not ready. Exiting...");
    Console.ReadLine();
    return;
}

while (true)
{
    Console.WriteLine("\n=== Musikkbibliotek ===");
    Console.WriteLine("1. Legg til ny sang");
    Console.WriteLine("2. Søk etter sanger");
    Console.WriteLine("3. Oppdater sang");
    Console.WriteLine("4. Slett sang");
    Console.WriteLine("5. Legg til ny spilleliste");
    Console.WriteLine("6. Søk etter spillelister");
    Console.WriteLine("7. Oppdater spilleliste");
    Console.WriteLine("8. Slett spilleliste");
    Console.WriteLine("9. Legg til sang i spilleliste");
    Console.WriteLine("10. Fjern sang fra spilleliste");
    Console.WriteLine("11. Vis sanger i spilleliste");
    Console.WriteLine("12. Avslutt");
    Console.Write("Velg et alternativ (1-12): ");
    var choice = Console.ReadLine()?.Trim();

    try
    {
        switch (choice)
        {
            case "1": CreateSong(musicRepo); break;
            case "2": SearchSongs(musicRepo); break;
            case "3": UpdateSong(musicRepo); break;
            case "4": DeleteSong(musicRepo); break;
            case "5": CreatePlayList(playListRepo); break;
            case "6": SearchPlayLists(playListRepo); break;
            case "7": UpdatePlayList(playListRepo); break;
            case "8": DeletePlayList(playListRepo); break;
            case "9": AddSongToPlayList(playListRepo, musicRepo); break;
            case "10": RemoveSongFromPlayList(playListRepo); break;
            case "11": ViewSongsInPlayList(playListRepo, musicRepo); break;
            case "12":
                Console.WriteLine("Avslutter programmet. Ha en fin dag!");
                Db.Close();
                Console.ReadLine();
                return;
            default: Console.WriteLine("Ugyldig valg, prøv igjen."); break;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine("Noe gikk galt prøv på nytt!");
    }
    
}

static void CreateSong(MusicRepo repo)
{
    Console.WriteLine("\n== Legg til ny sang ==");
    var songDto = ReadSongDetails();
    if (songDto == null)
    {
        Console.WriteLine("Ugyldige data. Avbryter opprettelse.");
        return;
    }

    var songDb = SongDto.Song_to_SongDB(songDto);
    var result = repo.InsertSongs([songDb]);
    Console.WriteLine(result.Result ? "Ok!" : "Kunne ikke legge til sang.");
}

static void SearchSongs(MusicRepo repo)
{
    Console.WriteLine("\n== Søk etter sanger ==");
    Console.WriteLine("Bruk gjerne komma hvis du har flere søkeord");
    Console.Write("Skriv inn søkeord (sjangre, stemning eller tagger): ");
    var term = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(term))
    {
        Console.WriteLine("Ingen søkekriterier oppgitt.");
        return;
    }
    var terms = term.Split(',');
    foreach (var i in terms)
    {
        Console.WriteLine(i);
    }
    var result = repo.FindSongs([..terms]);
    if (!result.Result)
    {
        Console.WriteLine("Ingen resultater funnet eller feil under søk.");
        return;
    }

    var songs = result.ReturnValues?.OfType<SongDb>().ToArray();
    if (songs?.Length == 0)
    {
        Console.WriteLine("Ingen resultater funnet. #2 ");
    }

    Console.WriteLine($"Funnet {songs?.Length} sang(er):");
    foreach (var s in songs ?? [])
        Console.WriteLine(s + "\n");
}

static void UpdateSong(MusicRepo repo)
{
    Console.WriteLine("\n== Oppdater sang ==");
    Console.Write("Skriv inn ID på sangen som skal oppdateres: ");
    if (!int.TryParse(Console.ReadLine(), out var id) || id <= 0)
    {
        Console.WriteLine("Ugyldig ID.");
        return;
    }

    var songDto = ReadSongDetails();
    if (songDto == null)
    {
        Console.WriteLine("Ugyldige data. Avbryter oppdatering.");
        return;
    }

    songDto.Id = id;
    var songDb = SongDto.Song_to_SongDB(songDto);
    var result = repo.UpdateSongs([songDb]);
    Console.WriteLine(result.Result ? "Ok!" : "Kunne ikke oppdatere sang.");
}

static void DeleteSong(MusicRepo repo)
{
    Console.WriteLine("\n== Slett sang ==");
    Console.Write("Skriv inn ID på sangen som skal slettes: ");
    if (!int.TryParse(Console.ReadLine(), out var id) || id <= 0)
    {
        Console.WriteLine("Ugyldig ID.");
        return;
    }

    var songDb = new SongDb { Id = id };
    var result = repo.DeleteSongs([songDb]);
    Console.WriteLine(result.Result ? "Ok!" : "Interne feil!");
}

static void CreatePlayList(PlayListRepo repo)
{
    Console.WriteLine("\n== Legg til ny spilleliste ==");
    Console.Write("Navn på spillelisten: ");
    var name = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Ugyldig navn. Avbryter opprettelse.");
        return;
    }

    var playListDb = new PlayListDb { Name = name };
    var result = repo.InsertPlayList([playListDb]);
    Console.WriteLine(result.Result ? "Ok!" : "Kunne ikke legge til spilleliste.");
}

static void SearchPlayLists(PlayListRepo repo)
{
    Console.WriteLine("\n== Søk etter spillelister ==");
    Console.Write("Skriv inn navn på spillelisten: ");
    var name = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Ingen navn oppgitt.");
        return;
    }

    var result = repo.GetPlayListByName(name);
    if (!result.Result)
    {
        Console.WriteLine("Ingen resultater funnet eller feil under søk.");
        return;
    }

    var playlists = result.ReturnValues?.OfType<PlayListDb>().ToArray();
    if (playlists?.Length == 0)
    {
        Console.WriteLine("Ingen spillelister funnet.");
        return;
    }

    Console.WriteLine($"Funnet {playlists?.Length} spilleliste(r):");
    foreach (var pl in playlists!)
        Console.WriteLine($"ID: {pl.Id}, Navn: {pl.Name}\n");
}

static void UpdatePlayList(PlayListRepo repo)
{
    Console.WriteLine("\n== Oppdater spilleliste ==");
    Console.Write("Skriv inn ID på spillelisten som skal oppdateres: ");
    if (!int.TryParse(Console.ReadLine(), out var id) || id <= 0)
    {
        Console.WriteLine("Ugyldig ID.");
        return;
    }

    Console.Write("Nytt navn på spillelisten: ");
    var name = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Ugyldig navn. Avbryter oppdatering.");
        return;
    }

    var playListDb = new PlayListDb { Id = id, Name = name };
    var result = repo.UpdatePlayList([playListDb]);
    Console.WriteLine(result.Result ? "Ok!" : "Kunne ikke oppdatere spilleliste.");
}

static void DeletePlayList(PlayListRepo repo)
{
    Console.WriteLine("\n== Slett spilleliste ==");
    Console.Write("Skriv inn ID på spillelisten som skal slettes: ");
    if (!int.TryParse(Console.ReadLine(), out var id) || id <= 0)
    {
        Console.WriteLine("Ugyldig ID.");
        return;
    }

    var playListDb = new PlayListDb { Id = id };
    var result = repo.DeletePlayList([playListDb]);
    Console.WriteLine(result.Result ? "Ok!" : "Kunne ikke slette spilleliste.");
}

static void AddSongToPlayList(PlayListRepo playListRepo, MusicRepo musicRepo)
{
    Console.WriteLine("\n== Legg til sang i spilleliste ==");
    Console.Write("Skriv inn navn på spillelisten: ");
    var userInpute = Console.ReadLine();
    if (string.IsNullOrEmpty(userInpute))
    {
        return;
    }

    Console.Write("Skriv inn ID på sangen: ");
    if (!int.TryParse(Console.ReadLine(), out var songId) || songId <= 0)
    {
        Console.WriteLine("Ugyldig sang-ID.");
        return;
    }

    var playList = playListRepo.GetPlayListByName(userInpute).ReturnValues?.OfType<PlayListDb>().FirstOrDefault();
    if (playList == null)
    {
        Console.WriteLine("Spillelisten finnes ikke.");
        return;
    }

    var song = musicRepo.GetSongById(songId).ReturnValues?.OfType<SongDb>().FirstOrDefault();
    if (song == null)
    {
        Console.WriteLine("Sangen finnes ikke.");
        return;
    }

    var canAdd = playListRepo.CanIAddThisSongToPlayList(song, new PlayListDb { Id = playList.Id, Name = playList.Name}, musicRepo);
    if (!canAdd.Result)
    {
        Console.WriteLine("Kan ikke legge til sangen i spillelisten (f.eks. finnes allerede eller sjangerkonflikt).");
        return;
    }

    var connection = new SongsInPlayListDb { PlayListId = playList.Id, SongId = song.Id };
    var result = playListRepo.InsertSongToPlayList([connection]);
    Console.WriteLine(result.Result ? "Ok!" : "Kunne ikke legge til sang i spilleliste.");
}

static void RemoveSongFromPlayList(PlayListRepo repo)
{
    Console.WriteLine("\n== Fjern sang fra spilleliste ==");
    Console.Write("Skriv inn ID på spillelisten: ");
    if (!int.TryParse(Console.ReadLine(), out var playListId) || playListId <= 0)
    {
        Console.WriteLine("Ugyldig spilleliste-ID.");
        return;
    }

    Console.Write("Skriv inn ID på sangen: ");
    if (!int.TryParse(Console.ReadLine(), out var songId) || songId <= 0)
    {
        Console.WriteLine("Ugyldig sang-ID.");
        return;
    }

    var connection = new SongsInPlayListDb { PlayListId = playListId, SongId = songId };
    var result = repo.DeleteSongToPlayList([connection]);
    Console.WriteLine(result.Result ? "Ok!" : "Kunne ikke fjerne sang fra spilleliste.");
}

static void ViewSongsInPlayList(PlayListRepo playListRepo, MusicRepo musicRepo)
{
    Console.WriteLine("\n== Vis sanger i spilleliste ==");
    Console.Write("Skriv inn navn til spillelisten: ");
    var userInpute = Console.ReadLine();
    if (string.IsNullOrEmpty(userInpute))
    {
        Console.WriteLine("Ugyldig spilleliste-navn.");
        return;
    }

    var playList = playListRepo.GetPlayListByName(userInpute).ReturnValues?.OfType<PlayListDb>().FirstOrDefault();
    if (playList == null)
    {
        Console.WriteLine("Spillelisten finnes ikke.");
        return;
    }

    var result = playListRepo.GetSongsFromPlayList(playList, musicRepo);
    if (!result.Result)
    {
        Console.WriteLine("Ingen sanger funnet eller feil under henting.");
        return;
    }

    var songs = result.ReturnValues?.OfType<SongDb>().ToArray();
    if (songs?.Length == 0)
    {
        Console.WriteLine("Ingen sanger i spillelisten.");
        return;
    }

    Console.WriteLine($"Funnet {songs?.Length} sang(er) i spillelisten:");
    foreach (var s in songs!)
        Console.WriteLine(s + "\n");
}

static Song? ReadSongDetails()
{
    try
    {
        var song = new Song();
        Console.Write("Navn: ");
        song.Name = Console.ReadLine()?.Trim() ?? throw new ArgumentException("Name is required");
        Console.Write("ArtistId: ");
        song.ArtistId = int.Parse(Console.ReadLine()?.Trim() ?? throw new ArgumentException("ArtistId is required"));
        Console.Write("AlbumId: ");
        song.AlbumId = int.Parse(Console.ReadLine()?.Trim() ?? throw new ArgumentException("AlbumId is required"));
        Console.Write("Sjangre (mellomrom-separert): ");
        song.Genres = Console.ReadLine()?
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .ToArray() ?? [];
        if (song.Genres.Length == 0) throw new ArgumentException("Genre is required");
        Console.Write("Beskrivelse: ");
        song.Description = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Tagger (komma-separert): ");
        song.Tags = Console.ReadLine()?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .ToArray() ?? [];
        if (song.Tags.Length == 0) throw new ArgumentException("Tags is required");
        return song;
    }
    catch
    {
        return null;
    }
}