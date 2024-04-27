using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class FavoritesDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string FavoritesID { get; set; } =  string.Empty;


        public string? UserId { get; set; }


        public int? ApplicationID { get; set; }
    }
}

