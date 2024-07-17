using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RzumeAPI.Models.Utilities;

namespace RzumeAPI.Models.DTO
{
	public class ApplicationDTO
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public string ApplicationID { get; set; } 

        public string? UserId { get; set; } 


        public string Position { get; set; } 

        public  string ApplicationDate { get; set; } 

        public List<string>? AdditionalDocuments { get; set; }

        public Status Status { get; set; }



        public int FavoritesID { get; set; }
    }
}

