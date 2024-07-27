using MusicPlayer.DAL;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.BLL.Services
{
    public class MusicPlaylistService
    {
        MusicPlayerContext _context;

        public void Add(PlaylistMusic pm)
        {
            _context = new();
            _context.PlaylistMusics.Add(pm);
            _context.SaveChanges();
        }

        public List<PlaylistMusic> GetAll()
        {
            _context = new();
            return _context.PlaylistMusics.ToList();
        }

        public void DeletePlaylistMusicByMusicId(int musicId)
        {
            _context = new();
            var pm = _context.PlaylistMusics.FirstOrDefault(pm => pm.MusicId == musicId);
            if(pm != null)
            {
                _context.PlaylistMusics.Remove(pm);
                _context.SaveChanges();
            }       
        }
    }
}
