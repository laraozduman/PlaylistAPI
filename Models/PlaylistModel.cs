public class PlaylistModel
{
    public long Id { get; set; }
    public List<SongModel> Songs { get; set; } = new List<SongModel>();
    public string PlaylistName { get; set; }
}    