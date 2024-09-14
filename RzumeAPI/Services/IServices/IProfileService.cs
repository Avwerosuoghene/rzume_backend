
using RzumeAPI.Models.Responses;
using RzumeAPI.Models.Requests;

namespace RzumeAPI.Services.IServices
{
    public interface IProfileService
    {

       
         Task<GenericResponse> OnboardingFirstStage(OnboardUserFirstStageRequest onboardRequestPayload, string userMail);
         Task<GenericResponse> OnboardingSecondStage(OnboardUserSecondStageRequest onboardRequestPayload, string userMail);
         Task<GenericResponse> OnboardingThirdStage(OnboardUserThirdStageRequest onboardRequestPayload, string userMail);
         Task<GenericResponse> OnboardingFourthStage(OnboardUserFourthStageRequest onboardRequestPayload, string userMail);

        Task<GenericResponse> RequestPasswordReset(RequestPasswordReset requestPasswordRequest, string clientSideBaseUrl);

         Task<GenericResponse> ResetPassword(ResetPassword resetPasswordPayload);

    };
}