using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class Otp
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string OtpID { get; set; } 

        [ForeignKey("User")]
        public required string UserId { get; set; } 

        public required string OtpValue { get; set; } 

        public bool IsConfirmed {get; set;}


        public DateTime ExpirationDate { get; set; }


        public required User User { get; set; }
    }


}

