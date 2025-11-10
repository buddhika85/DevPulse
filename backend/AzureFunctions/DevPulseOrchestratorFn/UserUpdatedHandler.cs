using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DevPulseOrchestratorFn
{
    public class UserUpdatedHandler
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;



        public UserUpdatedHandler(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = loggerFactory.CreateLogger<UserUpdatedHandler>();
            _config = config;

        }


        /// <summary>
        /// Azure Function triggered by Service Bus topic 'user-updates'.
        /// Deserializes incoming message, extracts UserId, and calls Orchestrator API to Invalidate dashboard cache.
        /// Logs message metadata for diagnostics and handles errors gracefully.
        /// </summary>
        [Function(nameof(UserUpdatedHandler))]
        public async Task Run(
                    [ServiceBusTrigger("user-updates", "orchestrator-sub", Connection = "ServiceBusConnection")]
                    ServiceBusReceivedMessage message,
                    ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Received message: {Message}", message);

            try
            {
                _logger.LogInformation("Message ID: {id}", message.MessageId);
                _logger.LogInformation("Message Body: {body}", message.Body);
                _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

                // Deserialize
                var payload = JsonSerializer.Deserialize<UserUpdatedPayload>(message.Body);
                if (payload?.UserId is not null)
                {
                    // read settings from appSettings.json
                    var baseUrl = _config["OrchestratorApi:BaseUrl"];
                    var endpoint = $"{baseUrl}/api/dashboard/invalidate/{payload.UserId}";

                    // ask OrchestratorApi to Invalidate cache as there is a user update
                    var response = await _httpClient.PostAsync(endpoint, null);

                    _logger.LogInformation("Cache invalidation triggered for UserId: {UserId}, Status: {StatusCode}",
                        payload.UserId, response.StatusCode);
                }
                else
                {
                    _logger.LogWarning("Invalid payload: missing UserId");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process UserUpdated message");
                throw;
            }
        }

    }


    public class UserUpdatedPayload
    {
        public required string UserId { get; set; }
    }

}
