namespace sigma_music;

public static class SongDto
{
    public static SongDb Song_to_SongDB(Song song)
    {
        return new SongDb()
        {
            Id = song.Id,
            Name = song.Name,
            AlbumId = song.AlbumId,
            ArtistId = song.ArtistId,
            Description = song.Description,
            Genres = string.Join(" ", song.Genres),
            Tags = string.Join(",", song.Tags)
        };
    }
    
    public static Song SongDb_to_Song(SongDb songDb)
    {
        return new Song()
        {
            Id = songDb.Id,
            Name = songDb.Name,
            AlbumId = songDb.AlbumId,
            ArtistId = songDb.ArtistId,
            Description = songDb.Description,
            Genres = songDb.Genres.Split(" "),
            Tags = songDb.Tags.Split(",")
        };
    }
}