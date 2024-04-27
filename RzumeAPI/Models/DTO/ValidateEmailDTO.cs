namespace RzumeAPI.Models.DTO
{
    public class ValidateEmailDTO
    {
        public  string Email { get; set; } = string.Empty;

    }

    public class ValidateEmailResponseDTO {
        public bool isValidated;
    }
}

