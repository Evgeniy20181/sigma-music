namespace sigma_music;

public static class SongDto
{
    public static SongDb Song_to_SongDB(Song song)
    {
        return new SongDb();
    }
    
    public static Song SongDb_to_Song(SongDb songDb)
    {
        return new Song();
    }
}