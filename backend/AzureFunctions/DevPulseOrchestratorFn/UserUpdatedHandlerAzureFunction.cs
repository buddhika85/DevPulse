using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedLib.DTOs.AzureServiceBusEvents;
using System.Net;
using System.Text.Json;

namespace DevPulseOrchestratorFn
{
    // An azure function
    public class UserUpdatedHandlerAzureFunction
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;



        public UserUpdatedHandlerAzureFunction(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = loggerFactory.CreateLogger<UserUpdatedHandlerAzureFunction>();
            _config = config;
        }

        /// <summary>
        /// This acts as a seprate azure function with in same Function app
        /// Scales in the same way as other azure fucnctions such as - UserUpdatedHandlerAzureFunction
        /// Used to check healthyness of azure function app
        /// </summary>
        /// <param name="req">HttpRequestData</param>
        /// <returns>HttpResponseData</returns>
        [Function("HealthCheckAzureFunction")]
        public async Task<HttpResponseData> Health([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("HealthCheckAzureFunction invoked.");
            try
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Healthy - {DateTime.UtcNow}");
                _logger.LogInformation("HealthCheckAzureFunction success at {Time}", DateTime.UtcNow);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HealthCheckAzureFunction failed at {Time}", DateTime.UtcNow);
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Health check failed: {ex.Message}");                
                return errorResponse;
            }
        }


        /// <summary>
        /// Azure Function triggered by Service Bus topic 'user-updates'.
        /// Deserializes incoming message, extracts UserId, and calls Orchestrator API to Invalidate dashboard cache.
        /// Logs message metadata for diagnostics and handles errors gracefully.
        /// </summary>
        [Function(nameof(UserUpdatedHandlerAzureFunction))]
        public async Task Run(
                    [ServiceBusTrigger("user-updates", "orchestrator-sub", Connection = "ServiceBusConnection")]
                    ServiceBusReceivedMessage message,
                    ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("UserUpdatedHandlerAzureFunction - Received message: {Message}", message);

            try
            {
                _logger.LogInformation("Message ID: {id}", message.MessageId);
                _logger.LogInformation("Message Body: {body}", message.Body);
                _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

                var payload = DeserializeMessage(message);

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

        /// <summary>
        /// A helper to Deserialize ServiceBusReceivedMessage
        /// </summary>
        /// <param name="message">ServiceBusReceivedMessage</param>
        /// <returns>BaseUserUpdatedPayload?</returns>
        /// <exception cref="ArgumentNullException">if updateField is unavailable</exception>
        private BaseUserUpdatedPayload? DeserializeMessage(ServiceBusReceivedMessage message)
        {
            // extract update field
            var doc = JsonDocument.Parse(message.Body);
            var updateField = doc.RootElement.GetProperty("UpdateField").ToString();


            if (string.IsNullOrWhiteSpace(updateField))
            {
                _logger.LogError("UserUpdatedHandlerAzureFunction - updateField not provided.");
                throw new ArgumentNullException("UserUpdatedHandlerAzureFunction - updateField not provided.");
            }

            // Deserialize
            BaseUserUpdatedPayload? payload = updateField switch
            {

                "DisplayName" => JsonSerializer.Deserialize<UserDisplayNameChangedAzServiceBusPayload>(message.Body),
                "Email" => JsonSerializer.Deserialize<UserEmailChangedAzServiceBusPayload>(message.Body),
                "Role" => JsonSerializer.Deserialize<UserRoleChangedAzServiceBusPayload>(message.Body),
                "All" => JsonSerializer.Deserialize<UserUpdatedAzServiceBusPayload>(message.Body),
                _ => JsonSerializer.Deserialize<UserUpdatedAzServiceBusPayload>(message.Body)
            };
            _logger.LogInformation("Message Body Deserialized: {DeserializedBody}", payload?.ToString());
            return payload;
        }
    }
}


/*
 
UserUpdatedAzServiceBusPayload

 {
  "UserId": "a1b2c3d4-e5f6-7890-abcd-1234567890ef",
  "UpdateField": "All",
  "Message": "User updated",
  "TimeStamp": "2025-11-13T04:00:00Z"
}



UserDisplayNameChangedAzServiceBusPayload

{
  "UserId": "a1b2c3d4-e5f6-7890-abcd-1234567890ef",
  "UpdateField": "DisplayName",
  "OldDisplayName": "John Doe",
  "NewDisplayName": "Jonathan D.",
  "Email": "jonathan@example.com",
  "Message": "User Display name updated",
  "TimeStamp": "2025-11-13T04:00:00Z"
}

UserEmailChangedAzServiceBusPayload

{
  "UserId": "a1b2c3d4-e5f6-7890-abcd-1234567890ef",
  "UpdateField": "Email",
  "PreviousEmail": "old@example.com",
  "NewEmail": "new@example.com",
  "Email": "new@example.com",
  "Message": "User Email updated",
  "TimeStamp": "2025-11-13T04:00:00Z"
}


UserRoleChangedAzServiceBusPayload

{
  "UserId": "a1b2c3d4-e5f6-7890-abcd-1234567890ef",
  "UpdateField": "Role",
  "PreviousRole": "User",
  "NewRole": "Admin",
  "Email": "admin@example.com",
  "Message": "User Role updated",
  "TimeStamp": "2025-11-13T04:00:00Z"
}
 
 */