namespace RzumeAPI.Models.DTO {

public class LoginResponseDTO
	{
		public UserDTO User { get; set; }
		public string Token { get; set; }

		public string Message {get; set;}

		public bool EmailConfirmed {get; set;}
	}

}