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
        public string ID { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool OnBoarded { get; set; }

        public byte? OnBoardingStage { get; set; }



    }


}

