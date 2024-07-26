using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.DAL;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.BLL.Services
{
    public class MusicPlaylistService
    {
        MusicPlayerContext _context = new();

        public void Add(PlaylistMusic pm)
        {
            _context.PlaylistMusics.Add(pm);
        }
    }
}
