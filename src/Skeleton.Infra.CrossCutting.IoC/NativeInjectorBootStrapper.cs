using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skeleton.Application.Interfaces;
using Skeleton.Application.Services;
using Skeleton.Domain.CommandHandlers;
using Skeleton.Domain.Commands.Customer;
using Skeleton.Domain.Core.Bus;
using Skeleton.Domain.Core.Events;
using Skeleton.Domain.EventHandlers;
using Skeleton.Domain.Events.Customer;
using Skeleton.Domain.Interfaces;
using Skeleton.Infra.CrossCutting.Bus;
using Skeleton.Infra.CrossCutting.Bus.Configurations;
using Skeleton.Infra.Data.Context;
using Skeleton.Infra.Data.EventSourcing;
using Skeleton.Infra.Data.Repository;
using Skeleton.Infra.Data.Repository.EventSourcing;

namespace Skeleton.Infra.CrossCutting.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Domain Bus Config
            services.AddOptions<MessageServiceBusConfig>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("MessageServiceBusConfig").Bind(settings);
            });

            // Domain Bus
            services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddScoped<IMessageHandler, MessageServiceBus>();

            // Application
            services.AddScoped<ICustomerAppService, CustomerAppService>();

            // Domain - Events

            // Customer
            services.AddScoped<INotificationHandler<CustomerRegisteredEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerUpdatedEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerRemovedEvent>, CustomerEventHandler>();

            // Domain - Commands

            // Customer
            services.AddScoped<IRequestHandler<RegisterNewCustomerCommand, object>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateCustomerCommand, ValidationResult>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveCustomerCommand, ValidationResult>, CustomerCommandHandler>();

            // Infra - Data
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<SkeletonContext>();

            // Infra - Data EventSourcing
            services.AddScoped<IEventStoreRepository, EventStoreSqlRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();
            services.AddScoped<EventStoreSqlContext>();
        }
    }
}