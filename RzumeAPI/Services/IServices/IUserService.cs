using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;


namespace RzumeAPI.Services.IServices
{
    public interface IUserService
    {



        Task<LoginResponse> Login(object loginRequestDTO);
        Task<bool> Logout(LogoutRequest logoutRequestDTO);

        Task<OtpPasswordResetRequestResponseDTO> InitiateOtpResetPassword(OtpPasswordResetRequestDTO passwordResetRequestModel);

        Task<RegisterUserResponse<ResultObject>> RegisterUserWithEmail(RegistrationRequest model,string clientSideBaseUrl);

        Task<GoogleSignupResponse> RegisterUserWithGoogle(GoogleSigninRequest googleRequest);

        Task<GetActiveUserResponse> GetActiveUser(string token);

        Task<ActivateUserAccountResponse> ActivateUserAccount(string token);




    };
}


