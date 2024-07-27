using Microsoft.EntityFrameworkCore;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.DAL.Repositories
{
    public class MusicRepository
    {
        MusicPlayerContext _context;

        public void Add(Music m)
        {
            _context = new();
            _context.Musics.Add(m);
            _context.SaveChanges();
        }

        public void Delete(Music music)
        {
            _context = new();
            _context.Musics.Remove(music);
            _context.SaveChanges();
        }

        public List<Music> GetAll()
        {
            _context = new();
            return _context.Musics.Include("User").ToList();
        }
    }
}
