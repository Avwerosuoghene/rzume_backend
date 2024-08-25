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
        public required string ApplicationID { get; set; } 

        public required string UserId { get; set; } 


        public required string Position { get; set; } 

        public required string ApplicationDate { get; set; } 

        public string JobLink { get; set; } = string.Empty;

        public string ResumeLink { get; set; } = string.Empty;


        public Status Status { get; set; }



    }
}

