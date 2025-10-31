public class PlaylistDto
{
    public long? Id { get; set; }
    public string PlaylistName { get; set; } = string.Empty;
    public List<SongDto>? Songs { get; set; }
}