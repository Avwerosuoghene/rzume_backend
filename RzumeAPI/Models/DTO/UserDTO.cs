using System;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class UserDTO
    {
         public UserDTO()
        {
            OnBoardingStage = 0;
        }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string UserName { get; set; } 

        public string Email { get; set; } 

        public bool OnBoarded { get; set; }

        public byte? OnBoardingStage { get; set; }

        public bool EmailConfirmed { get; set; }



    }


}

