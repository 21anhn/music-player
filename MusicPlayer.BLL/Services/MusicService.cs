using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.DAL;
using MusicPlayer.DAL.Models;
using MusicPlayer.DAL.Repositories;

namespace MusicPlayer.BLL.Services
{
    public class MusicService
    {

        private MusicRepository _repo = new();
        private MusicPlaylistService _msService = new();

        public void AddMusic(Music m)
        {
            _repo.Add(m);
        }

        public void DeleteMusic(Music music)
        {
            _repo.Delete(music);
        }

        public List<Music> GetAllMusics()
        {
            return _repo.GetAll();
        }

        // Method to get all musics not in the current playlist
        public List<Music> GetMusicsNotInPlaylist(List<Music> musics, int currentPlaylistId)
        {

            // Get all musics in the current playlist
            var musicsInPlaylist = _msService.GetAll()
                                              .Where(ms => ms.PlaylistId == currentPlaylistId)
                                              .Select(ms => ms.MusicId)
                                              .ToList();

            // Filter out musics that are already in the current playlist
            var musicsNotInPlaylist = musics.Where(music => !musicsInPlaylist.Contains(music.MusicId))
                                               .ToList();

            return musicsNotInPlaylist;
        }
    }
}
