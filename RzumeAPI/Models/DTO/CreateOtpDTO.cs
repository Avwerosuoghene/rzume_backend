using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class CreateOtpDTO
    {

        public string UserId { get; set; }
        public string OtpValue { get; set; }

        public bool IsConfirmed { get; set; }



        public DateTime ExpirationDate { get; set; }
    }
}

