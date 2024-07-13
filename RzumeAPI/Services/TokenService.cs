using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RzumeAPI.Models;

namespace RzumeAPI.Services
{
    public class TokenService
    {

        private IConfiguration _configuration;



        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public TokenServiceResponse ValidateToken(string token)
        {
            var secretKey = _configuration["ApiSettings:Secret"];
            try
            {
                var key = Encoding.ASCII.GetBytes(secretKey!);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claimsPrincipal = tokenHandler.ValidateToken(token, validations, out var tokenSecure);
                var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var userMail = userIdClaim?.Value;

                return new TokenServiceResponse()
                {
                    UserMail = userMail,
                    Message = "Success"
                };

            }   
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine($"Token expired: {ex.Message}");

                return new TokenServiceResponse()
                {
                    UserMail = null,
                    Message = "Token Expired"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new TokenServiceResponse()
                {
                    UserMail = null,
                    Message = ex.Message
                };
            }
        }

        // public string? GetTokenByUserId(string userId) {

        // }


        public string GenerateToken(string userId, string userMail, DateTime expiration)
        {
            var secretKey = _configuration["ApiSettings:Secret"];

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
                Expires = expiration,

                //Describes signin creddentials
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var uniqueGenerateToken = tokenHandler.WriteToken(token);
            return uniqueGenerateToken;
        }

    }
}