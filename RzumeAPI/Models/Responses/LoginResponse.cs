using RzumeAPI.Models.DTO;

namespace RzumeAPI.Models.Responses;

public class LoginResponse
{
    public UserDTO? User { get; set; }
    public string? Token { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }
}

public class LoginResponseContent {
    public string Token { get; set; } = string.Empty;

   public required UserDTO User { get; set; }
}
