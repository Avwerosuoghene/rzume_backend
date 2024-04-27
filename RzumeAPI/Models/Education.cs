using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class Education
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string EducationID { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;


        public  string InstitutionName { get; set; } = string.Empty;

        public  string CourseOfStudy { get; set; } = string.Empty;

        // public string? Grade { get; set; }

        public DateTime GraduationDate { get; set; }


        public User? User { get; set; }

    }
}

