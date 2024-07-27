using MusicPlayer.DAL.Models;

namespace MusicPlayer.DAL.Repositories
{
    public class UserRepository
    {
        private MusicPlayerContext _context;

        public bool RegisterUser(User user)
        {
            _context = new();
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public User? GetOne(string username, string password)
        {
            _context = new();
            return _context.Users.FirstOrDefault(x => x.Username.ToLower() == username.ToLower() && x.Password == password);
        }

        public List<User> GetAll()
        {
            _context = new();
            return _context.Users.ToList();
        }
    }
}
