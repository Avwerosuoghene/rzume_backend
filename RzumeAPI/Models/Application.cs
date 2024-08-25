using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RzumeAPI.Models.Utilities;

namespace RzumeAPI.Models
{
    public class Application
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string ApplicationID { get; set; }

        public required string UserId { get; set; }
        public required string CompanyID { get; set; }


        public required string Position { get; set; }

        public required string ApplicationDate { get; set; } 

   
        public string JobLink { get; set; } = string.Empty;
        public string ResumeLink { get; set; } = string.Empty;

        public Status Status { get; set; } = Status.Wishlist;


        public User? User { get; set; }
        public Company? Company { get; set; }
    }


}

