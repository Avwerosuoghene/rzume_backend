using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class CompanyDTO
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CompanyID { get; set; } 



        public  string Name { get; set; } 

        public string? Email { get; set; }

        public string? Website { get; set; }

        public string? Contact { get; set; }



    }
}

