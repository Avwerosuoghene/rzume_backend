using RzumeAPI.Data;

namespace RzumeAPI.Services
{
    public class UserService
    {

        private ApplicationDbContext _db;


        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool IsUniqueUser(string email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return true;
            }

            return false;
        }

    }
}