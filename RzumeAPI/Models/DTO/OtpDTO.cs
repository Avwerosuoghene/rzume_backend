

namespace RzumeAPI.Models.DTO
{
    public class OtpDTO
    {

        public string OtpID { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string OtpValue { get; set; } = string.Empty;

        public bool IsConfirmed { get; set; } = false;



        public DateTime ExpirationDate { get; set; }
    }

    public class CreateOtpDTO
    {

        public string UserId { get; set; } = string.Empty;
        public string OtpValue { get; set; } = string.Empty;

        public bool IsConfirmed { get; set; } = false;



        public DateTime ExpirationDate { get; set; }
    }

    public class GenerateOtpDTO
    {
        public string Email { get; set; } = string.Empty;

        public string Purpose { get; set; } = string.Empty;

    }


    public class GenerateOtpResponseDTO
    {
        public bool IsSuccess { get; set; } 



    }

    public class OtpPasswordResetRequestResponseDTO
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;



    }

    public class OtpPasswordResetRequestDTO
    {
        public string Email { get; set; } = string.Empty;

        public string OtpValue { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;


    }

    public class OtpValidationDTO
    {
        public string Email { get; set; } = string.Empty;

        public string OtpValue { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

    }

    public class OtpValidationResponseDTO
    {

        public bool IsValid { get; set; }

        public string Message { get; set; } = string.Empty;


    }
}

