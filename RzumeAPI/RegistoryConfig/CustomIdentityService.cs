using Microsoft.AspNetCore.Identity;
using RzumeAPI.Data;
using RzumeAPI.Models;
namespace RzumeAPI.RegistoryConfig;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        

        return services;
    }
}


