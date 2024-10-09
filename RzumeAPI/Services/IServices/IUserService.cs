using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;


namespace RzumeAPI.Services.IServices
{
    public interface IUserService
    {



        Task<APIServiceResponse<ResultObject>> Login(object loginRequestDTO);
        Task<APIServiceResponse<ResultObject>> Logout(LogoutRequest logoutRequestDTO);

        Task<OtpPasswordResetRequestResponseDTO> InitiateOtpResetPassword(OtpPasswordResetRequestDTO passwordResetRequestModel);

        Task<RegisterUserResponse<ResultObject>> RegisterUserWithEmail(RegistrationRequest model,string clientSideBaseUrl);

        Task<GoogleSignupResponse> RegisterUserWithGoogle(GoogleSigninRequest googleRequest);

         Task<APIServiceResponse<ResultObject>> GetActiveUser(string token);

        Task<APIServiceResponse<ResultObject>> ActivateUserAccount(string token);




    };
}


