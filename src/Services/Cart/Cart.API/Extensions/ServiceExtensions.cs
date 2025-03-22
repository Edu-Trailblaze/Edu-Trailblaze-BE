using EventBus.Messages.Events;
using EventBus.Messages.Interfaces;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;

namespace Cart.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var eventBusSetting = configuration.GetSection(nameof(EventBusSetting)).Get<EventBusSetting>();
            services.AddSingleton(eventBusSetting);
            return services;
        }

        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            var setting = services.GetOptions<EventBusSetting>("EventBusSetting");
            if (setting == null || string.IsNullOrEmpty(setting.HostAddress))
            {
                throw new ArgumentNullException("EventBus is not configuration");
            }
            
            var mqConnection = new Uri(setting.HostAddress);
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(mqConnection);
                });
                x.AddRequestClient<GetCourseRequest>(new Uri("queue:course-queue"));
            });
            //services.AddMassTransitHostedService();
        }
    }
}
