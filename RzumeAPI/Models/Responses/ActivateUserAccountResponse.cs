using RzumeAPI.Models.DTO;

namespace RzumeAPI.Models.Responses;


public class ActivateUserAccountResponse
{
    public string Message { get; set; } = string.Empty;

    public UserDTO? User { get; set; }

    public string? Token { get; set; } = string.Empty;
}