
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Models.Responses;

public class RegisterUserResponse
{
    public UserDTO? User { get; set; }
    public string? Message { get; set; } = string.Empty;
}