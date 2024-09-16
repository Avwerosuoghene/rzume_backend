
using RzumeAPI.Models;
using RzumeAPI.Models.Responses;

namespace RzumeAPI.Services.IServices;
    public interface ITokenService
    {
         TokenServiceResponse ValidateToken(string token);

         Task<string> GenerateToken(User user, DateTime expiration, string tokenName);

         Task<string> SendTokenEmailValidation(User user, string clientSideBaseUrl);

         Task<GetUserFromTokenResponse> GetUserFromToken(string token);
    }