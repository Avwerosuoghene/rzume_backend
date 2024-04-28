

namespace RzumeAPI.Models.DTO
{
    public class OtpDTO
    {

        public string OtpID { get; set; } 

        public string UserId { get; set; } 

        public string OtpValue { get; set; } 

        public bool IsConfirmed { get; set; } = false;



        public DateTime ExpirationDate { get; set; }
    }

    public class CreateOtpDTO
    {

        public string UserId { get; set; } 
        public string OtpValue { get; set; } 

        public bool IsConfirmed { get; set; } = false;



        public DateTime ExpirationDate { get; set; }
    }

    public class GenerateOtpDTO
    {
        public string Email { get; set; } 

        public string Purpose { get; set; } 

    }


    public class GenerateOtpResponseDTO
    {
        public bool IsSuccess { get; set; } 



    }

    public class OtpPasswordResetRequestResponseDTO
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } 



    }

    public class OtpPasswordResetRequestDTO
    {
        public string Email { get; set; } 

        public string OtpValue { get; set; } 

        public string Password { get; set; } 


    }

    public class OtpValidationDTO
    {
        public string Email { get; set; } 

        public string OtpValue { get; set; } 

        public string Password { get; set; } 

    }

    public class OtpValidationResponseDTO
    {

        public bool IsValid { get; set; }

        public string Message { get; set; } 


    }
}

