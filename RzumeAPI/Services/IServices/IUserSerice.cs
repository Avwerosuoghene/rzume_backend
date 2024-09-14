using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;


namespace RzumeAPI.Services.IServices
{
    public interface IUserService
    {

        Task<RegisterUserResponse> Register(object registrationRequestDTO, string? clientSideBaseUrl);

        Task<LoginResponse> Login(object loginRequestDTO);
        Task<bool> Logout(LogoutRequest logoutRequestDTO);

        Task<OtpPasswordResetRequestResponseDTO> InitiateOtpResetPassword(OtpPasswordResetRequestDTO passwordResetRequestModel);

         Task<GetActiveUserResponse> GetActiveUser(string token);


         Task<string> SendTokenEmailValidation(User user, string clientSideBaseUrl);

        Task<ActivateUserAccountResponse> ActivateUserAccount( string token);


        
    };
}


