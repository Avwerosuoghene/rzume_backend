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
        public string ID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool OnBoarded { get; set; }



        // public string? Location { get; set; }

        // [ForeignKey("EducationID")]
        // public ICollection<Education>? Education { get; set; }

        // [ForeignKey("ExperienceID")]
        // public ICollection<Experience>? Experience { get; set; }

        // [ForeignKey("FileId")]
        // public ICollection<UserFile>? UserFiles { get; set; }

        // public string? Skills { get; set; }


        // [ForeignKey("FavoritesID")]
        // public Favorites? Favorites { get; set; }


        // public byte[]? ProfilePicture { get; set; }


        // [ForeignKey("ApplicationID")]
        // public ICollection<Application>? Applications { get; set; }

        // public string? Bio { get; set; }

        public byte? OnBoardingStage { get; set; }



    }


}

