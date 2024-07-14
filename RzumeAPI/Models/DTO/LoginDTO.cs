using System;
namespace RzumeAPI.Models.DTO
{
	public class LoginRequestDTO
	{
          public string UserName { get; set; }  = string.Empty;

        public string? Password { get; set; } 
    }


public class LoginResponseDTO
	{
		public  UserDTO? User { get; set; }
		public string? Token { get; set; } 

		public string Message {get; set;}  = string.Empty;

		public bool EmailConfirmed {get; set;}
	}
}

