using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Identity;

namespace Skeleton.Infra.CrossCutting.Identity.Configurations
{
    public static class WebAppIdentityConfig
    {
        public static void AddWebAppIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Default EF Context for Identity (inside of the NetDevPack.Identity)
            services.AddIdentityEntityFrameworkContextConfiguration(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Skeleton.Infra.CrossCutting.Identity")));

            // Default Identity configuration from NetDevPack.Identity
            services.AddIdentityConfiguration();
        }
    }
}