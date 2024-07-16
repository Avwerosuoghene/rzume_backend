using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class Otp
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OtpID { get; set; } 

        [ForeignKey("User")]
        public string UserId { get; set; } 

        public string OtpValue { get; set; } 

        public bool IsConfirmed {get; set;}


        public DateTime ExpirationDate { get; set; }


        public User User { get; set; }
    }


}

