namespace RzumeAPI.Models.Requests;

public class RegistrationRequest
{
    public required string Email { get; set; }

    public required string Password { get; set; }

}

public class GoogleSigninRequest {
    public required string Email { get; set; }


    public required string GoogleId { get; set; } 
}