namespace sigma_music;

public class PlayListDb
{
    public int Id { get; set; }
    public string Name { get; set; } = "Unknown";
}

public class SongsInPlayListDb
{
    public int SongId { get; set; }
    public int PlayListId { get; set; }
}