using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class OtpDTO
    {

        public string OtpID { get; set; }

        public string UserId { get; set; }

        public string OtpValue { get; set; }

        public bool IsConfirmed { get; set; }



        public DateTime ExpirationDate { get; set; }
    }

     public class CreateOtpDTO
    {

        public string UserId { get; set; }
        public string OtpValue { get; set; }

        public bool IsConfirmed { get; set; }



        public DateTime ExpirationDate { get; set; }
    }

    public class GenerateOtpDTO
    {
        public  string Email { get; set; }

        public string Purpose {get; set;}

    }

     public class OtpPasswordResetRequestResponseDTO
    {
        public bool isSuccess { get; set; }

        public string message { get; set; }



    }

     public class OtpPasswordResetRequestDTO
    {
        public string Email { get; set; }

        public string OtpValue { get; set; }

        public string Password {get; set;}


    }

     public class OtpValidationDTO
    {
        public  string Email { get; set; }

        public  string OtpValue { get; set; }

         public string Password { get; set; } 

    }

     public class OtpValidationResponseDTO 
    {

        public bool isValid { get; set; }

        public string message { get; set; }

 
    }
}

