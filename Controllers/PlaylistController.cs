using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Playlist.Models;

namespace Playlist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistContext _context;

        public PlaylistController(PlaylistContext context)
        {
            _context = context;
        }

        [HttpPost("playlist")]
        public async Task<ActionResult<PlaylistModel>> CreatePlaylist([FromBody] PlaylistModel playlist)
        {
            if (playlist.PlaylistName == null || playlist.PlaylistName.Length == 0)
            {
                return BadRequest("Invalid playlist name.");
            }

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylists), new { id = playlist.Id }, playlist);
        }

        [HttpGet("playlists")]
        public async Task<ActionResult<IEnumerable<PlaylistModel>>> GetPlaylists()
        {
            if (_context.Playlists == null)
            {
                return NotFound();
            }
            var playlists = await _context.Playlists.Include(p => p.Songs).ToListAsync();
            return Ok(playlists);
        }

        [HttpDelete("playlists/{id}")]
        public async Task<IActionResult> DeletePlaylist(long id)
        {
            var playlist = await _context.Playlists.Include(p => p.Songs).FirstOrDefaultAsync(p => p.Id == id);
            if (playlist == null)
            {
                return NotFound();
            }
             _context.Songs.RemoveRange(playlist.Songs);
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/playlist/{playlistId}
        [HttpPut("/playlist/{playlistId}")]
        public async Task<IActionResult> UpdatePlaylistName(long playlistId, [FromBody] string newName)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null)
                return NotFound($"Playlist {playlistId} not found.");

            playlist.PlaylistName = newName;
            await _context.SaveChangesAsync();
            return NoContent();
}


        // GET: api/TodoItems
        //retrieve all songs in a playlist
        [HttpGet("playlist/{playlistId}/songs")]
        public async Task<ActionResult<IEnumerable<SongModel>>> GetAllSongs(long playlistId)
        {
          if (_context.Songs == null)
          {
              return NotFound();
          }

          var songs = await _context.Songs.Include(s => s.Playlist)
              .Where(s => s.PlaylistId == playlistId)
              .ToListAsync();

          if (songs == null || songs.Count == 0)
          {
              return NotFound();
          }

          return songs;
        }

        // GET: api/TodoItems/5
        // retrieve a specific song by id
        [HttpGet("{id}")]
        public async Task<ActionResult<SongModel>> GetSong(long id)
        {
            var song = await _context.Songs.Include(s => s.Playlist).FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                return NotFound();
            }

            return Ok(song);
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SongModel>> PostSong(SongModel song)
        {
            if (!_context.Playlists.Any(p => p.Id == song.PlaylistId))
            {
                return BadRequest("Invalid Playlist, please enter a valid playlist.");
            }

            if (song.SongName == null || song.SongName.Length == 0)
            {
                return Problem("Need name for song");
            }

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSong), new { id = song.Id }, song);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        // [Authorize]
        public async Task<IActionResult> DeleteSong(long id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("playlist/{playlistId}/songs")]
        public async Task<IActionResult> DeleteAllSongsInPlaylist(long playlistId)
        {
            var songs = await _context.Songs.Where(s => s.PlaylistId == playlistId).ToListAsync();
            if (songs == null || songs.Count == 0)
            {
                return NotFound();
            }

            _context.Songs.RemoveRange(songs);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SongModel>>> SearchSongInPlaylist([FromQuery] String search, [FromQuery] long playlistId)
        {
            if (search == null || search.Length == 0)
            {
                return BadRequest("Search string cannot be null");
            }

            if (_context.Songs == null)
            {
                return NotFound();
            }

            var songs = await _context.Songs
                .Where(s => s.SongName != null && s.SongName.Contains(search) && s.PlaylistId == playlistId)
                .ToListAsync();

            if (songs.Count == 0)
            {
                return NotFound("No songs found matching the search");
            }
            return Ok(songs);
        }


        private bool SongExists(long id)
        {
            return (_context.Songs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}


// şu anda playlist ekleme silme güncelleme işlemleri yok sadece şarkılar için var
// birden fazla playlist olabilir ve bir playlistin birden fazla şarkısı olabilir
// şarkılar playlistlere foreign key ile bağlı
// playlistteki bütün şarkıları silme ucu eklenecek
