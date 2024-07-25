using Microsoft.EntityFrameworkCore;
using MusicPlayer.DAL.Models;
using MusicPlayer.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.BLL.Services
{
    public class UserService
    {
        private UserRepository _repo = new();

        public User? Authenticate(string username, string password) => _repo.GetOne(username, password);

        public bool Register(string fullName, string username, string password)
        {
            User user = new();
            user.FullName = fullName;
            user.Username = username;
            user.Password = password;

            var existingUser = _repo.RegisterUser(user);

            return true;
        }
    }
}
