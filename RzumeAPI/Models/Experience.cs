using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class Experience
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string ExperienceID { get; set; } 

        public required string UserId { get; set; }


        public string? Industry { get; set; }

        public  string? Company { get; set; }

        public required string Designation { get; set; } 

        public  DateTime StartDate { get; set; }

        public  DateTime EndDate { get; set; }

        public User? User { get; set; }
    }

}

