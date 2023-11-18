﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Configurations;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Repository;
using RzumeAPI.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

var configuration = builder.Configuration;


// Add services to the container.

builder.Services.AddControllers(option =>
{

}).AddNewtonsoftJson();

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


// AddDefaultTokenProviders enables us to use the email token generator
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<SMTPConfigModel>(configuration.GetSection("SMTPConfig"));

builder.Services.Configure<IdentityOptions>(options => {
    options.SignIn.RequireConfirmedEmail = true;
} );

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//This registers our mapping config
builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IEmailRepository, EmailRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}








app.UseAuthorization();

app.MapControllers();

app.Run();


