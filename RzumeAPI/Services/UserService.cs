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

        public string GenerateDefaultPassword()
        {
            const int passwordLength = 16;
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            var passwordChars = new char[passwordLength];
            for (int i = 0; i < passwordLength; i++)
            {
                passwordChars[i] = validChars[random.Next(validChars.Length)];
            }
            return new string(passwordChars);
        }

    }
}