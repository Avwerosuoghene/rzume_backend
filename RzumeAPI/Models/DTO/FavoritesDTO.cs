using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class FavoritesDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string FavoritesID { get; set; } 


        public required string UserId { get; set; }


        public int? ApplicationID { get; set; }
    }
}

