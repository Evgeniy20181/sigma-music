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
        Id = 2, Name = "Billie Jean", ArtistId = 2, AlbumId = 2, Genres = "Pop",
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
//musicRepo.DebugToggle();
//musicRepo.InsertSongs(songs);
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
    Console.WriteLine("5. Avslutt");
    Console.Write("Velg et alternativ (1-5): ");
    var choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1": CreateSong(musicRepo); break;
        case "2": SearchSongs(musicRepo); break;
        case "3": UpdateSong(musicRepo); break;
        case "4": DeleteSong(musicRepo); break;
        case "5":
            Console.WriteLine("Avslutter programmet. Ha en fin dag!");
            return;
        default: Console.WriteLine("Ugyldig valg, prøv igjen."); break;
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
    foreach (var s in songs??[])
        Console.WriteLine(s.ToString()+"\n");
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
    var result = repo.DeleteSongs(new[] { songDb });
    Console.WriteLine(result.Result ? "Ok!" : "Interne feil!");
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
            .ToArray()??[];
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