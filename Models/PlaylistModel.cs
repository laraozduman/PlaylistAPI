public class PlaylistModel
{
    public long Id { get; set; }
    public List<SongModel>? Songs { get; set; }
    public string PlaylistName { get; set; }
}