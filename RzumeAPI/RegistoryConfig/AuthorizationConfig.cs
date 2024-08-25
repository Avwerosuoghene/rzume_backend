using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace RzumeAPI.RegistoryConfig;


public static class AuthorizationServiceExtensions
{
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {

        var jwtConfig = configuration.GetSection("JwtConfig");
        var secretKey = jwtConfig["Secret"];
        var issuer = jwtConfig["ValidIssuer"];
        var audience = jwtConfig["ValidAudiences"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            if (secretKey is null || issuer is null || audience is null)
            {
                throw new ApplicationException("Jwt is not set in the configuration");
            }
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

            };
        }
        );
    }
}