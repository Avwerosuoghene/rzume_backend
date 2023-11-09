using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class Education
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string EducationID { get; set; }

        public string UserId { get; set; }


        public  string InstitutionName { get; set; }

        public  string CourseOfStudy { get; set; }

        public string? Grade { get; set; }

        public DateTime YearOfGraudation { get; set; }


        public User? User { get; set; }

    }
}

