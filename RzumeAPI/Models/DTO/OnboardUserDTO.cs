using System;
namespace RzumeAPI.Models.DTO
{
    public class OnboardUserRequestDTO
    {
        public int OnBoardingStage { get; set; } 

        public dynamic OnboardUserPayload { get; set; } = null!;

        public string UserMail { get; set; } = string.Empty;


    }



    public class OnboardUserResponseDTO
    {
        public string isValid { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

    }

    public class OnboardUserFirstStageRequestDTO
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;


    }

    public class OnboardUserSecondStageRequestDTO
    {

        public string FileName { get; set; }  =  string.Empty;
        public string FileBytes { get; set; } =  string.Empty;

        public string FileCat {get; set;} =  string.Empty;

    }

    public class OnboardUserThirdStageRequestDTO
    {

        public List<EducationDTO> EducationList { get; set; } = new List<EducationDTO>();



    }

}