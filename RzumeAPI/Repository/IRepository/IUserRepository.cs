using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;


namespace RzumeAPI.Repository.IRepository
{
    public interface IUserRepository
    {



        Task<User?> GetUserByEmailAsync (string email);

        Task<User> UpdateAsync(User user);

        
    };
}


