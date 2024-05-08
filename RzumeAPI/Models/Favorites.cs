using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class Favorites
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string FavoritesID { get; set; } 

        public User? User { get; set; }
        public string UserId { get; set; }

        public ICollection<Application>? Applications { get; set; }

        public string? ApplicationID { get; set; }

    }
}

