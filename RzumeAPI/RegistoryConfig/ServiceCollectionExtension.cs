
using RzumeAPI.Services;
using RzumeAPI.Services.IServices;

namespace RzumeAPI.RegistoryConfig {
public static class ServiceCollectionExtension
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<FileService>();
        services.AddScoped<OtpService>();
        services.AddScoped<TokenService>();
        services.AddScoped<IUserService, UserService>();
    }
}

}

