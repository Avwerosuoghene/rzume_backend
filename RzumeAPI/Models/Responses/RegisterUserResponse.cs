
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Models.Responses;

public class RegisterUserResponse
{
    public User? User { get; set; }
    public string? Message { get; set; } = string.Empty;
}