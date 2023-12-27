using System;
using Microsoft.AspNetCore.Identity;

namespace RzumeAPI.Models
{
    public class User : IdentityUser
    {

        public string? Name { get; set; }


        public string? Location { get; set; }


        //[ForeignKey("EducationID")]
        public ICollection<Education>? Education { get; set; }

        //[ForeignKey("ExperienceID")]
        public ICollection<Experience>? Experience { get; set; }


        public string? Skills { get; set; }


        //[ForeignKey("FavoritesID")]
        public Favorites? Favorites { get; set; }


        public byte[]? ProfilePicture { get; set; }


        //[ForeignKey("ApplicationID")]
        public ICollection<Application>? Applications { get; set; }

        public string? Bio { get; set; }

        public  bool Verified { get; set; }
    }

}

