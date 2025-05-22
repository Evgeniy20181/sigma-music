namespace sigma_music;

public class SongDb
{
    public int Id { get; set; }
    public string Name { get; set; } = "Unknown";
    public int ArtistId { get; set; }
    public int AlbumId { get; set; }
    public string Genres { get; set; } = "";
    public string Description { get; set; } = "Unknown";
    public string Tags { get; set; } = "";


    public override string ToString()
    {
        return $"#{Id} Song: {Name}\n" +
               $"Artist ID: {ArtistId}, Album ID: {AlbumId}\n" +
               $"Genres: {Genres}\n" +
               $"Description: {Description}\n" +
               $"Tags: {Tags}";
    }

}