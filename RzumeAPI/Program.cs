using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Configurations;
using RzumeAPI.Data;
using RzumeAPI.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using RzumeAPI.Services;
using RzumeAPI.RegistoryConfig;
using RzumeAPI.Options;
using Serilog;
using RzumeAPI.Models.Utilities;
using Serilog.Formatting.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOption>(builder.Configuration.GetSection(DatabaseOption.SectionName));
builder.Services.Configure<BaseUrlOptions>(builder.Configuration.GetSection(BaseUrlOptions.SectionName));

builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, config) =>
{
    config.Enrich.FromLogContext().WriteTo.File(
     "logs/log.txt", rollingInterval: RollingInterval.Day
    ).WriteTo.Console(new JsonFormatter()).ReadFrom.Configuration(context.Configuration);
});

var logger = new LoggerConfiguration().WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/log.txt"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90).CreateLogger();
builder.Logging.AddSerilog(logger);



builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

var configuration = builder.Configuration;

builder.Services.AddControllers(option =>
{

}).AddNewtonsoftJson();

builder.Services.AddControllers();


builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddCustomIdentity();

builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.Configure<SMTPConfigModel>(configuration.GetSection("SMTPConfig"));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});


builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

    }
).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = configuration.GetSection("Authentication:Google:ClientId").Value;
    options.ClientSecret = configuration.GetSection("Authentication:Google:ClientSecret").Value;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddRouting(options => {
    options.LowercaseUrls = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

ServiceCollectionExtension.RegisterServices(builder.Services);

RepositoryCollectionExtension.RegisterRepository(builder.Services);

OptionsCollectionExtension.RegisterOptions(builder.Services, builder.Configuration);

builder.Services.AddScoped<FileService>();

var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}








app.UseAuthorization();

app.MapControllers();

app.Run();


