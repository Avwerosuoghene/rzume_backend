using RzumeAPI.Models.DTO;

namespace RzumeAPI.Models.Responses;

 public class GetActiveUserResponse
    {
        public UserDTO? User { get; set; }
        public string Message { get; set; } = string.Empty;
    }
