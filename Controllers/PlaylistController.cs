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

        // POST: api/playlist/playlist
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

        // GET: api/playlist/playlists
        [HttpGet("playlists")]
        public async Task<ActionResult<IEnumerable<PlaylistModel>>> GetPlaylists()
        {
            if (_context.Playlists == null)
            {
                return NotFound();
            }

            try
            {
                var playlists = await _context.Playlists.Include(p => p.Songs).ToListAsync();
                var playlistDtos = playlists.Select(p => new PlaylistDto
                {
                    Id = p.Id,
                    PlaylistName = p.PlaylistName,
                    Songs = p.Songs?.Select(s => new SongDto
                    {
                        Id = s.Id,
                        SongName = s.SongName,
                        Artist = s.Artist,
                        Duration = s.Duration
                    }).ToList()
                }).ToList();

                return Ok(playlistDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/playlist/playlists/{id}
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
        [HttpPut("{playlistId}")]
        public async Task<IActionResult> UpdatePlaylistName(long playlistId, [FromBody] string newName)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null)
                return NotFound($"Playlist {playlistId} not found.");

            playlist.PlaylistName = newName;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        // GET: api/playlist/{playlistId}/songs
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

        // GET: api/playlist/5
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

        // POST: api/playlist
        [HttpPost]
        public async Task<ActionResult<SongModel>> PostSong([FromBody] SongDto songDto)
        {
            if (!_context.Playlists.Any(p => p.Id == songDto.PlaylistId))
            {
                return BadRequest("Invalid Playlist, please enter a valid playlist.");
            }

            if (songDto.SongName == null || songDto.SongName.Length == 0)
            {
                return Problem("Need name for song");
            }
            var song = new SongModel
            {
                SongName = songDto.SongName,
                Artist = songDto.Artist,
                Duration = songDto.Duration,
                PlaylistId = songDto.PlaylistId
            };
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSong), new { id = song.Id }, song);
        }

        // DELETE: api/playlist/5
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

        // DELETE: api/playlist/playlist/{playlistId}/songs
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


    }
}


// şu anda playlist ekleme silme güncelleme işlemleri yok sadece şarkılar için var
// birden fazla playlist olabilir ve bir playlistin birden fazla şarkısı olabilir
// şarkılar playlistlere foreign key ile bağlı
// playlistteki bütün şarkıları silme ucu eklenecek
