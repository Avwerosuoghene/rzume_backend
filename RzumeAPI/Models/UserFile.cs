using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class UserFile
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string FileId { get; set; } 

        public required string FileName { get; set; } 

        public required string FileCategory { get; set; } 

        public required string FileBytes { get; set; } 

        public required string UserId { get; set; } 

        public required User User { get; set; }
    }

}

