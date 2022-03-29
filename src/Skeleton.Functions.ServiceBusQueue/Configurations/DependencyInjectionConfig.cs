using System;
using Microsoft.Extensions.DependencyInjection;
using Skeleton.Infra.CrossCutting.IoC;

namespace Skeleton.Functions.ServiceBusQueue.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}