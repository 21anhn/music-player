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
    }
}
