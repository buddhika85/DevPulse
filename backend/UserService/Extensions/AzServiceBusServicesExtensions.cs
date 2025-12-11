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
            RegisterServiceBusClient(services);
            RegisterServiceBusPublisher(services);

            return services;
        }

        private static void RegisterServiceBusPublisher(IServiceCollection services)
        {
            // Register custom publisher class as a singleton.
            // This class wraps the ServiceBusClient and provides a clean interface for publishing messages.
            // It will be injected wherever IServiceBusPublisher is needed (e.g., in event handlers).
            services.AddSingleton<IServiceBusPublisher, AzureServiceBusPublisher>();
        }

        private static void RegisterServiceBusClient(IServiceCollection services)
        {
            // Register ServiceBusClient as a singleton.
            // This client is used to send messages to Azure Service Bus.
            // It reads the connection string from configuration.
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config["AzureServiceBusConnectionString"];

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Missing AzureServiceBusConnectionString in configuration");

                return new ServiceBusClient(connectionString, new ServiceBusClientOptions
                {
                    TransportType = ServiceBusTransportType.AmqpWebSockets          // Required for Azure App Service: avoids blocked AMQP ports
                });
            });
        }
    }
}
