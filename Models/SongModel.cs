public class SongModel
{
    public long Id { get; set; }
    public string SongName { get; set; }
    public string? Duration { get; set; }
    public string? Artist { get; set; }
    [ForeignKey("Playlist")]
    public long PlaylistId { get; set; }
    public PlaylistModel Playlist { get; set; }
}