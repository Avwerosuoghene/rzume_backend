
using RzumeAPI.Services;

namespace RzumeAPI.RegistoryConfig {
public static class ServiceCollectionExtension
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

