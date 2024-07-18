using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class EducationDTO
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string EducationID { get; set; }


        public required string  UserId { get; set; }

        public required string InstitutionName { get; set; } 

        public required string CourseOfStudy { get; set; } 

        public DateTime GraduationDate { get; set; }

    }
}

