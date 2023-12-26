using System;
using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Repository.IRepository
{
    public interface IUserRepository
    {

        Task<UserDTO> Register(RegistrationDTO registrationRequestDTO);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<IdentityResult> ConfirmEmail(string uid, string token);

        Task GenerateEmailConfirmationToken(User user);

        Task<User> GetUserByEmailAsync (string email);
    }
}


