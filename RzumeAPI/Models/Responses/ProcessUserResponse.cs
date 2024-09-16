
namespace RzumeAPI.Models.Responses;

public class ProcessUserResponse
{
    public User? User { get; set; }
    public string? Message { get; set; } = string.Empty;
}