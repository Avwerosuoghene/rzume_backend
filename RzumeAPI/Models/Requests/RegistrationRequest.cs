namespace RzumeAPI.Models.Requests;

public class RegistrationRequest
{
    public required string Email { get; set; }

    public required string Password { get; set; }

}

public class GoogleSigninRequest {
    public required string Email { get; set; }


    public required string Name { get; set; } 

    public string GivenName { get; set; } = string.Empty;

    public string FamilyName { get; set; } = string.Empty;
}