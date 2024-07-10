using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RzumeAPI.Services
{
    public class TokenService
    {


        private static string? secretKey;

        public TokenService(IConfiguration configuration)
        {
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }


        public static string GenerateToken(string userId, string userMail)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey!);



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Describes what our token will contain
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.Email,userMail)
                }),

                //Describes Token Expiration
                Expires = DateTime.UtcNow.AddDays(7),

                //Describes signin creddentials
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var uniqueGenerateToken = tokenHandler.WriteToken(token);
            return uniqueGenerateToken;
        }

    }
}