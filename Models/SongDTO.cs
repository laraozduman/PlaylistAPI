public class SongDto
{
    public long? Id { get; set; }
    public string SongName { get; set; } = string.Empty;
    public string? Duration { get; set; }
    public string? Artist { get; set; }
    public long PlaylistId { get; set; }
    public PlaylistDto? Playlist { get; set; }

}