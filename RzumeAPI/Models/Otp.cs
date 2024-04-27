using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace RzumeAPI.Models
{
    public class Otp
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OtpID { get; set; } = string.Empty;

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        public string OtpValue { get; set; } = string.Empty;

        public bool IsConfirmed {get; set;}


        public DateTime ExpirationDate { get; set; }


        public User User { get; set; }  =  new User();
    }


}

