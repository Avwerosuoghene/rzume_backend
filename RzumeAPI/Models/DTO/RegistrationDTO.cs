namespace RzumeAPI.Models.DTO
{
    public class RegistrationDTO
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class RegistrationResponseDTO
    {
        public bool IsCreated { get; set; }

    }
}

