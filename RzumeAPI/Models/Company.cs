using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CompanyID { get; set; } = string.Empty;

        public  string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Website { get; set; }

        public string? Contact { get; set; }


    }
}

