namespace RzumeAPI.Models.Responses;


public class ActivateUserAccountResponse
{
    public string Message { get; set; } = string.Empty;

    public bool AccountActivated { get; set; }
}