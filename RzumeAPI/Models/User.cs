using Microsoft.AspNetCore.Identity;

namespace RzumeAPI.Models
{
    public class User : IdentityUser
    {

        public User()
        {
            OnBoardingStage = 0;
        }

        public required string  Name { get; set; }


        public string Location { get; set; } = string.Empty;


        public ICollection<Education> Education { get; set; } = [];

        public ICollection<Experience> Experience { get; set; } =[];


        public ICollection<Skill> Skills { get; set; } = [];



        public ICollection<UserFile> UserFiles { get; set; } = [];

        public string GoogleId { get; set; } = string.Empty;



        public byte[]? ProfilePicture { get; set; }


        public ICollection<Application> Applications { get; set; } = [];

        public string? Bio { get; set; }

        public bool OnBoarded { get; set; }

        public byte? OnBoardingStage { get; set; }


    }


   
   



 

}

