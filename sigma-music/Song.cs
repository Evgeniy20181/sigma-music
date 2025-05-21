namespace sigma_music;

public class Song
{
    public int Id { get; set; }
    public string Name { get; set; } = "Unknown";
    public int ArtistId { get; set; }
    public int AlbumId { get; set; }
    public string[] Genres { get; set; } = [];
    public string Description { get; set; } = "Unknown";
    public string[] Tags { get; set; } = [];
}