
using RzumeAPI.Repository;
using RzumeAPI.Services;
using RzumeAPI.Services.IServices;

namespace RzumeAPI.RegistoryConfig
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<FileService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IProfileService, ProfileService>();

            services.AddScoped<IEmailService, EmailService>();

        }
    }

}

