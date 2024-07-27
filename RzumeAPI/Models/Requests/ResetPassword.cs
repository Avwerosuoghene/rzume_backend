namespace RzumeAPI.Models.Requests;


public class ResetPassword
{
    public required string Email { get; set; }
    public required string Password { get; set; }

    public required string ResetToken {get; set;}

}

