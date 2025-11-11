using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace UserService.Infrastructure.Messaging
{
    public class AzureServiceBusPublisher : IServiceBusPublisher
    {
        private readonly ServiceBusClient _client;          // Client used to create senders for topics
        private readonly ILogger<AzureServiceBusPublisher> _logger;

        public AzureServiceBusPublisher(ServiceBusClient client, ILogger<AzureServiceBusPublisher> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Publishes a message to the specified topic
        /// </summary>
        /// <param name="topicName">Azure Service Bus Topic</param>
        /// <param name="payload">payload to publish to Azure service bus topic</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task PublishAsync(string topicName, object payload, CancellationToken cancellationToken)
        {
            try
            {
                // Create a sender for the given topic
                var sender = _client.CreateSender(topicName);

                // Serialize the payload object to JSON
                var json = JsonSerializer.Serialize(payload);

                // Create a Service Bus message with the JSON payload
                var message = new ServiceBusMessage(json);

                // Send the message asynchronously to the topic
                await sender.SendMessageAsync(message, cancellationToken);
           
                _logger.LogInformation("Published message to topic '{Topic}' with payload: {Payload}", topicName, json);
            }
            catch (ServiceBusException sbEx)
            {
                _logger.LogError(sbEx, "Service Bus error while publishing to topic '{Topic}': {Message}", topicName, sbEx.Message);
                throw; // optionally rethrow or wrap in a custom exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while publishing to topic '{Topic}': {Message}", topicName, ex.Message);
                throw;
            }

        }

    }
}
