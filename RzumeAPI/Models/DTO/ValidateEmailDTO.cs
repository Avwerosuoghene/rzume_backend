namespace RzumeAPI.Models.DTO
{
    public class ValidateEmailDTO
    {
        public  string Email { get; set; }

    }

    public class ValidateEmailResponseDTO {
        public bool isValidated;
    }
}

