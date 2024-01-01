using System;
using Microsoft.AspNetCore.Identity;

namespace RzumeAPI.Models
{
    public class User : IdentityUser
    {

        public string? Name { get; set; }


        public string? Location { get; set; }


        public ICollection<Education>? Education { get; set; }

        public ICollection<Experience>? Experience { get; set; }


        public string? Skills { get; set; }


        public Favorites? Favorites { get; set; }


        public byte[]? ProfilePicture { get; set; }


        public ICollection<Application>? Applications { get; set; }

        public string? Bio { get; set; }

    }

}

