using System;
namespace RzumeAPI.Models.DTO
{
    public class OnboardUserRequestDTO
    {
        public int OnBoardingStage { get; set; } 

        public dynamic OnboardUserPayload { get; set; } = null!;

        public string UserMail { get; set; } 


    }



    public class OnboardUserResponseDTO
    {
        public string isValid { get; set; } 

        public string FirstName { get; set; } 
        public string LastName { get; set; } 

    }

    public class OnboardUserFirstStageRequestDTO
    {

        public string UserName { get; set; }  = string.Empty;


    }

    public class OnboardUserSecondStageRequestDTO
    {

        public string FileName { get; set; }  
        public string FileBytes { get; set; } 

        public string FileCat {get; set;} 

    }

    public class OnboardUserThirdStageRequestDTO
    {

        public List<EducationDTO> EducationList { get; set; } = new List<EducationDTO>();



    }

    public class OnboardUserFourthStageRequestDTO
    {

        public List<ExperienceDTO> ExperienceList { get; set; } = new List<ExperienceDTO>();



    }

}