using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class ExperienceDTO
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ExperienceID { get; set; } 


        public string UserId { get; set; } 

        public string? Industry { get; set; }

        public  string? Company { get; set; }

        public  string Designation { get; set; } 

        public  DateTime StartDate { get; set; }

        public  DateTime EndDate { get; set; }


    }
}

