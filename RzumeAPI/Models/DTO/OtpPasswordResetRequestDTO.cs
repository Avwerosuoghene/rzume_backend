using System;
namespace RzumeAPI.Models.DTO
{
    public class OtpPasswordResetRequestDTO
    {
        public string Email { get; set; }

        public string OtpValue { get; set; }

        public string Password {get; set;}


    }
}

