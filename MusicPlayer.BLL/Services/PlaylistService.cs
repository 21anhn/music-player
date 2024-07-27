using MusicPlayer.DAL.Models;
using MusicPlayer.DAL.Repositories;

namespace MusicPlayer.BLL.Services
{
    public class PlaylistService
    {
        private PlaylistRepository _repo = new();
        private MusicPlaylistService _msService = new();
        private MusicService _musicService = new();

        public void AddPlaylist(Playlist playlist)
        {
            _repo.Add(playlist);
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _repo.GetAll();
        }

        public List<Music> GetMusicsByPlaylistId(int playlistId)
        {
            //Danh sách ở bảng MQH N-N
            var msPlaylist = _msService.GetAll().Where(msPl => msPl.PlaylistId == playlistId).ToList();
            var musicIds = msPlaylist.Select(ms => ms.MusicId).ToList(); //Danh sách musicId của laylistId đó
            return _musicService.GetAllMusics().Where(music => musicIds.Contains(music.MusicId)).ToList();
        }

        public void DeletePlaylist(Playlist playlist)
        {
            _repo.Delete(playlist);
        }

        public void UpdatePlaylist(Playlist currentPlaylist)
        {
            _repo.Update(currentPlaylist);
        }
    }
}
