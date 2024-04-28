using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class UserFile
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string FileId { get; set; } 

        public string FileName { get; set; } 

        public string FileCategory { get; set; } 

        public string FileBytes { get; set; } 

        public string UserId { get; set; } 

        public User User { get; set; }
    }

}

