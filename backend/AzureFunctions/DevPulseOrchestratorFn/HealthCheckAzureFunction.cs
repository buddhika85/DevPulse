using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DevPulseOrchestratorFn;

public class HealthCheckAzureFunction
{    
    private readonly ILogger _logger;
    private readonly IConfiguration _config;

    public HealthCheckAzureFunction(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, IConfiguration config)
    {        
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
}