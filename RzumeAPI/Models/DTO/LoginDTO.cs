using System;
namespace RzumeAPI.Models.DTO
{
	public class LoginRequestDTO
	{
          public string UserName { get; set; } =  string.Empty;

        public string Password { get; set; } =  string.Empty;
    }


public class LoginResponseDTO
	{
		public  UserDTO? User { get; set; }
		public string Token { get; set; } =  string.Empty;

		public string Message {get; set;} =  string.Empty;

		public bool EmailConfirmed {get; set;}
	}
}

