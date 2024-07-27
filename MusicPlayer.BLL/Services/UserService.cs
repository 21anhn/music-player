﻿using MusicPlayer.DAL.Models;
using MusicPlayer.DAL.Repositories;

namespace MusicPlayer.BLL.Services
{
    public class UserService
    {
        private UserRepository _userRepo = new();
        private MusicRepository _musicRepo = new();
        private PlaylistRepository _plRepo = new();

        public User? Authenticate(string username, string password) => _userRepo.GetOne(username, password);

        public bool Register(string fullName, string username, string password)
        {
            User user = new();
            user.FullName = fullName;
            user.Username = username;
            user.Password = password;

            //Check username tồn tại hay chưa
            if (_userRepo.GetAll().FirstOrDefault(x => x.Username == username) != null)
            {
                return false;
            }

            return _userRepo.RegisterUser(user);
        }

        public List<Music> GetAllMusicsByUsername(string username)
        {
            var musics = _musicRepo.GetAll();
            var userMusics = musics.Where(m => m.User.Username == username).ToList();

            return userMusics;
        }

        public List<Playlist> GettAllPlaylistByUsername(string username)
        {
            var playlists = _plRepo.GetAll();
            return playlists.Where(p => p.User.Username == username).ToList();
        }
    }
}
