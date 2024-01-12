namespace RzumeAPI.Models.DTO
{
    public class RegistrationDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class RegistrationResponseDTO
    {
        public bool IsCreated { get; set; }

    }
}

