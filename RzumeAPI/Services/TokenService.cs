using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RzumeAPI.Models;
using RzumeAPI.Models.Responses;
using RzumeAPI.Models.Utilities;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Services.IServices;

namespace RzumeAPI.Services
{
    public class TokenService(IConfiguration configuration, UserManager<User> userManager,         IUserRepository userRepo
): ITokenService
    {

        private readonly IUserRepository _userRepo = userRepo;

        private IConfiguration _configuration = configuration;

        private readonly UserManager<User> _userManager = userManager;

        public TokenServiceResponse ValidateToken(string token)
        {
            var secretKey = _configuration["JwtConfig:Secret"];
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

           public async Task<GetUserFromTokenResponse> GetUserFromToken(string token)
        {
            TokenServiceResponse tokenServiceResponse = ValidateToken(token);
            string? userMail = tokenServiceResponse.UserMail;
            if (userMail == null)
            {
                return new GetUserFromTokenResponse()
                {
                    User = null,
                    Message = tokenServiceResponse.Message
                };
            }

            if (tokenServiceResponse.Message == TokenStatMsg.TokenExpired)
            {


                return new GetUserFromTokenResponse()
                {
                    User = null,
                    Message = TokenStatMsg.ActivationTokenActive
                };
            }

            var user = await _userRepo.GetUserByEmailAsync(userMail);
            if (user == null)
            {
                return new GetUserFromTokenResponse()
                {
                    User = null,
                    Message = UserStatMsg.NotFound
                };
            }

            return new GetUserFromTokenResponse()
            {
                User = user,
                Message = UserStatMsg.Found
            };
        }



        public async Task<string> GenerateToken(User user, DateTime expiration, string tokenName)
        {
            var secretKey = _configuration["JwtConfig:Secret"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey!);



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Describes what our token will contain
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id),
                    new Claim(ClaimTypes.Email,user.Email!)
                }),

                Expires = expiration,

                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var uniqueGenerateToken = tokenHandler.WriteToken(token);
            await _userManager.SetAuthenticationTokenAsync(user, tokenName, $"{tokenName}Token", uniqueGenerateToken);

            return uniqueGenerateToken;
        }


    }
}