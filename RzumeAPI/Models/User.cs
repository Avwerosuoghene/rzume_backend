using System;
using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Models
{
    public class User : IdentityUser
    {

        public User()
        {
            OnBoardingStage = 0;
        }

        public string? Name { get; set; }


        public string? Location { get; set; }


        public ICollection<Education>? Education { get; set; }

        public ICollection<Experience>? Experience { get; set; }


        public string? Skills { get; set; }


        public Favorites? Favorites { get; set; }

        public ICollection<UserFile>? UserFiles { get; set; }



        public byte[]? ProfilePicture { get; set; }


        public ICollection<Application>? Applications { get; set; }

        public string? Bio { get; set; }

        public bool OnBoarded { get; set; }

        public byte? OnBoardingStage { get; set; }


    }

    public class GetActiveUserResponse
    {
        public UserDTO User { get; set; }
        public string Message { get; set; } 
    }

}

