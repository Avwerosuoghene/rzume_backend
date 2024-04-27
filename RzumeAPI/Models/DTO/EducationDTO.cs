using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class EducationDTO
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string EducationID { get; set; } =  string.Empty;


        public string? UserId { get; set; }

        public string InstitutionName { get; set; } =  string.Empty;

        public string CourseOfStudy { get; set; } =  string.Empty;

        // public string? Grade { get; set; }



        public DateTime GraduationDate { get; set; }

    }
}

