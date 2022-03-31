using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Identity;
using NetDevPack.Identity.Data;
using NetDevPack.Identity.Jwt;

namespace Skeleton.Infra.CrossCutting.Identity.Configurations
{
    public static class ApiIdentityConfig
    {
        public static void AddApiIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Default EF Context for Identity (inside of the NetDevPack.Identity)
            services.AddIdentityEntityFrameworkContextConfiguration(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Skeleton.Infra.CrossCutting.Identity")));

            // Default Identity configuration from NetDevPack.Identity
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<NetDevPackAppDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<CustomIdentityErrorDescriber>();

            services.Configure<IdentityOptions>(options =>
            {
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(30));

            // Default JWT configuration from NetDevPack.Identity
            services.AddJwtConfiguration(configuration, "AppSettings");
        }
    }
}