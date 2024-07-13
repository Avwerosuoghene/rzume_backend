using RzumeAPI.Data;
using RzumeAPI.Models;

namespace RzumeAPI.Services
{
    public class UserService
    {

        private ApplicationDbContext _db;


        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public User? userExists(string email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return null;
            }

            return user;
        }

    }
}