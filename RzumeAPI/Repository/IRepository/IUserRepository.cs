using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Responses;


namespace RzumeAPI.Repository.IRepository
{
    public interface IUserRepository
    {

        Task<RegisterUserResponse> Register(RegistrationDTO registrationRequestDTO, string clientSideBaseUrl);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<bool> Logout(LogoutRequestDTO logoutRequestDTO);

          Task<OtpPasswordResetRequestResponseDTO> InitiateOtpResetPassword(OtpPasswordResetRequestDTO passwordResetRequestModel);
        Task<IdentityResult> ConfirmEmail(string uid, string token);

        // Task GenerateEmailConfirmationToken(User user);

        Task<User> GetUserByEmailAsync (string email);

        Task<User> UpdateAsync(User user);

         Task<GetActiveUserResponse> GetActiveUser(string token);


         Task<string> SendTokenEmailValidation(User user, string clientSideBaseUrl);

        Task<ActivateUserAccountResponse> ActivateUserAccount( string token);
    };
}


