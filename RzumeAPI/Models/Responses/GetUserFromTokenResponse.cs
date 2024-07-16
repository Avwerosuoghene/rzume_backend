namespace RzumeAPI.Models.Responses;

 public class GetUserFromTokenResponse
    {
        public User? User { get; set; }
        public string Message { get; set; } = string.Empty;
    }