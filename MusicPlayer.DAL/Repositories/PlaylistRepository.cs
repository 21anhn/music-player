using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.DAL.Repositories
{
    public class PlaylistRepository
    {
        private MusicPlayerContext _context;

        public void Add(Playlist playlist)
        {
            _context = new();
            _context.Playlists.Add(playlist);
            _context.SaveChanges();
        }

        public List<Playlist> GetAll()
        {
            _context = new();
            return _context.Playlists.Include(p => p.PlaylistMusics).Include(p => p.User).ToList();
        }

    }
}
