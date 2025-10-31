using Microsoft.EntityFrameworkCore;

namespace Playlist.Models;

public class PlaylistContext : DbContext
{
    public PlaylistContext(DbContextOptions<PlaylistContext> options)
        : base(options)
    {
    }

    public DbSet<PlaylistDto> PlaylistDtos { get; set; } = null!;
    public DbSet<SongDto> SongDtos { get; set; } = null!;
    public DbSet<PlaylistModel> Playlists { get; set; } = null!;
    public DbSet<SongModel> Songs { get; set; } = null!;
}