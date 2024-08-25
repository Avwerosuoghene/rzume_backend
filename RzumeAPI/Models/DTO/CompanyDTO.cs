using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class CompanyDTO
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string CompanyID { get; set; } 

        public required string Name { get; set; } 

        public string Email { get; set; } = string.Empty;

        public string Website { get; set; } = string.Empty;

        public string Industry { get; set; }  = string.Empty;



    }
}

