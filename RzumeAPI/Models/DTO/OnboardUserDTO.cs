using System;
namespace RzumeAPI.Models.DTO
{
    public class OnboardUserRequestDTO
    {
        public int OnBoardingStage { get; set; }

        public dynamic OnboardUserPayload { get; set; }

        public string UserMail { get; set; }


    }



    public class OnboardUserResponseDTO
    {
        public string isValid { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }

    }

    public class OnboardUserFirstStageRequestDTO
    {

        public String FirstName { get; set; }
        public String LastName { get; set; }


    }

    public class OnboardUserSecondStageRequestDTO
    {

        public string FileName { get; set; }
        public string FileBytes { get; set; }

        public FileCategory FileCat {get; set;}



    }

}