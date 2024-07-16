

namespace RzumeAPI.Models
{

    
        public class Token
        {
            public int UserId { get; set; }
            public string LoginProvider { get; set; } = string.Empty;
            public string Name { get; set; }= string.Empty;
            public string Value { get; set; } = string.Empty;
        }

          public static class TokenNames
    {
        public const string SignUp = "SignUp";
        public const string Login = "Login";
    }


      public static class TokenStatMsg {

        public const string TokenExpired = "Token Expired";
        public const string ActivationTokenActive = "Signup token still active, kindly check your mail";

        public const string ActivationTokenSent = "Kindly check your mail for your activation token";
      }
    



}

