using Microsoft.EntityFrameworkCore;
using MusicPlayer.DAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayer.DAL.Repositories
{
    public class UserRepository
    {
        private readonly MusicPlayerContext _context;

        public UserRepository(MusicPlayerContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return false;

         
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username);

            if (existingUser != null)
                return false;

        
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
