using System.Reflection;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Skeleton.Functions.ServiceBusQueue;
using Skeleton.Functions.ServiceBusQueue.Configurations;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Skeleton.Functions.ServiceBusQueue
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var context = builder.GetContext();

            // Setting DBContexts
            builder.Services.AddDatabaseConfiguration(context.Configuration);

            // Adding MediatR for Domain Events and Notifications
            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

            // .NET Native DI Abstraction
            builder.Services.AddDependencyInjectionConfiguration();
        }
    }
}