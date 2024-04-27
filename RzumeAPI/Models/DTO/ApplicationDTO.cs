using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
	public class ApplicationDTO
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public string ApplicationID { get; set; } =  string.Empty;

        public string? UserId { get; set; } =  string.Empty;


        public string Position { get; set; } =  string.Empty;

        public  string ApplicationDate { get; set; } =  string.Empty;

        public List<string>? AdditionalDocuments { get; set; }

        public Status Status { get; set; }



        public int FavoritesID { get; set; }
    }
}

