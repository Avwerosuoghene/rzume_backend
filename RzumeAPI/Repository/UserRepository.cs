
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Data;
using RzumeAPI.Models;

using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class UserRepository(ApplicationDbContext db) : IUserRepository
    {
        private ApplicationDbContext _db = db;




        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.ApplicationUsers
                            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> UpdateAsync(User user)
        {



            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }




}

