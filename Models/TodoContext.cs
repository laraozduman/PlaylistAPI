using Microsoft.EntityFrameworkCore;

namespace Playlist.Models;

public class PlaylistContext : DbContext
{
    public PlaylistContext(DbContextOptions<PlaylistContext> options)
        : base(options)
    {
    }

    public DbSet<Playlist> Playlists { get; set; } = null!;
    public DbSet<Song> Songs { get; set; } = null!;
}