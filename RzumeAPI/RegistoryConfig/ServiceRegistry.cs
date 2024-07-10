
using RzumeAPI.Services;

namespace RzumeAPI.RegistoryConfig {
public static class ServiceRegistry
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<FileService>();
        services.AddScoped<OtpService>();
        services.AddScoped<TokenService>();
        services.AddScoped<UserService>();
    }
}

}

