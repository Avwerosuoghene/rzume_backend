
namespace RzumeAPI.Models.DTO
{
    public class UserDTO
    {

        public required string UserName { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public required string Email { get; set; }

        public bool OnBoarded { get; set; }

        public byte OnBoardingStage { get; set; } = 0;

        public bool EmailConfirmed { get; set; }


        public string? GoogleId { get; set; }



    }


}

