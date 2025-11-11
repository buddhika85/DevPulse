using Azure.Messaging.ServiceBus;
using UserService.Infrastructure.Messaging;

namespace UserService.Extensions
{
    public static class AzServiceBusServicesExtensions
    {
        /// <summary>
        /// Registers Azure Service Bus client for publishing messages.
        /// Reads connection string from configuration key 'ServiceBusConnection'.
        /// </summary>
        public static IServiceCollection InjectAzureServiceBusServices(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config["ServiceBusConnection"];

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Missing ServiceBusConnection in configuration");

                return new ServiceBusClient(connectionString);
            });

            return services;
        }

    }
}
