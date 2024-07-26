using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.DAL.Models;
using MusicPlayer.DAL.Repositories;

namespace MusicPlayer.BLL.Services
{
    public class PlaylistService
    {
        private PlaylistRepository _repo = new();

        public void AddPlaylist(Playlist playlist)
        {
            _repo.Add(playlist);
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _repo.GetAll();
        }
    }
}
