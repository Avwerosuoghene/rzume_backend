using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace RzumeAPI.Models
{
    public class Application
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  string ApplicationID { get; set; } = string.Empty;

        public string? UserId { get; set; }


        public  string Position { get; set; } = string.Empty;

        public  string ApplicationDate { get; set; } = string.Empty;

        [NotMapped]
        public List<string>? AdditionalDocuments { get; set; }

        public Status Status { get; set; }

        public Favorites? Favorites { get; set; }

        public string? FavoritesID { get; set; }


        public User? User { get; set; }
    }


}

