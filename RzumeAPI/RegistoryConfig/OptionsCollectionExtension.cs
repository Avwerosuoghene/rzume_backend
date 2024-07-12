
using RzumeAPI.Options;

namespace RzumeAPI.RegistoryConfig;
public static class OptionsCollectionExtension
{
    public static IServiceCollection RegisterOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOption>(configuration.GetSection(DatabaseOption.SectionName));
        services.Configure<BaseUrlOptions>(configuration.GetSection(BaseUrlOptions.SectionName));
        services.Configure<BaseUrlOptions>(configuration.GetSection(ApiOptions.SectionName));

        return services;
    }
}