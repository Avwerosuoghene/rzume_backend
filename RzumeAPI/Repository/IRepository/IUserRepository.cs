using System;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);

        Task<UserDTO> Register(RegistrationDTO registrationRequestDTO);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    }
}


