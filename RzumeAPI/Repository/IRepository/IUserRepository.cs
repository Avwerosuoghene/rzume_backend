using RzumeAPI.Models;

namespace RzumeAPI.Repository.IRepository
{
    public interface IUserRepository
    {



        Task<User?> GetUserByEmailAsync (string email);

        Task<User> UpdateAsync(User user);

        
    };
}


