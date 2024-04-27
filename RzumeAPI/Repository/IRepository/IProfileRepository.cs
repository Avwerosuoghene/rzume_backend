using System;
using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Repository.IRepository
{
    public interface IProfileRepository
    {

       
         Task<GenericResponseDTO> OnboardingFirstStage(OnboardUserFirstStageRequestDTO onboardRequestPayload, string userMail);
         Task<GenericResponseDTO> OnboardingSecondStage(OnboardUserSecondStageRequestDTO onboardRequestPayload, string userMail);
         Task<GenericResponseDTO> OnboardingThirdStage(OnboardUserThirdStageRequestDTO onboardRequestPayload, string userMail);
    };
}


