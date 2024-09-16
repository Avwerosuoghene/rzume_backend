using RzumeAPI.Repository;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.RegistoryConfig {
    public static class RepositoryCollectionExtension
{
    public static void RegisterRepository(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUserFileRepository, UserFileRepository>();


        services.AddScoped<IOtpRepository, OtpRepository>();

        services.AddScoped<IEducationRepository, EducationRepository>();

        services.AddScoped<IExperienceRepository, ExperienceRepository>();


        services.AddScoped<IUtilityRepository, UtilityRepository>();

        services.AddScoped<ICompanyRepository, ComapanyRepository>();
    }
}
}
