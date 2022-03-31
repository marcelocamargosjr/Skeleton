using System.Reflection;
using System.Text.Json.Serialization;
using MediatR;
using NetDevPack.Identity;
using NetDevPack.Identity.User;
using Skeleton.Infra.CrossCutting.Identity.Configurations;
using Skeleton.Services.Api.Configurations;

const string allowSpecificOrigins = "allowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var env = hostingContext.HostingEnvironment;

    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
});

// Add services to the container.

// Cors Config
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.AddPolicy(allowSpecificOrigins, policyBuilder =>
        {
            policyBuilder.WithOrigins("https://example.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    }
    else
    {
        options.AddPolicy(allowSpecificOrigins, policyBuilder =>
        {
            policyBuilder.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }
});

// WebAPI Config
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Swagger Config
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration(builder.Environment.EnvironmentName);

// Setting DBContexts
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// ASP.NET Identity Settings & JWT
builder.Services.AddApiIdentityConfiguration(builder.Configuration);

// Interactive AspNetUser (logged in)
// NetDevPack.Identity dependency
builder.Services.AddAspNetUserConfiguration();

// AutoMapper Settings
builder.Services.AddAutoMapperConfiguration();

// Adding MediatR for Domain Events and Notifications
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// .NET Native DI Abstraction
builder.Services.AddDependencyInjectionConfiguration();

// Problem Details Config
builder.Services.AddProblemDetailsModelStateConfiguration();

// Client URI Config
builder.Services.Configure<ClientUriConfig>(builder.Configuration.GetSection("ClientUriConfig"));

// Sendinblue Config
builder.Services.Configure<SendinblueConfig>(builder.Configuration.GetSection("SendinblueConfig"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwaggerSetup();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(allowSpecificOrigins);

// NetDevPack.Identity dependency
app.UseAuthConfiguration();

app.MapControllers();

app.Run();