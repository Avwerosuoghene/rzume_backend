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


        public required string Position { get; set; }

        public required string ApplicationDate { get; set; } 

        [NotMapped]
        public List<string> AdditionalDocuments { get; set; } = [];

        public Status Status { get; set; }


        public User? User { get; set; }
    }


}

