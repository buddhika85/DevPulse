using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net;

namespace DevPulseOrchestratorFn;

/// <summary>
/// This Time trriggered Azure Function warms up microservices by sending HEAD request to Health endpoint up each microservice periodicaly.
/// Settings can be configured by below environments variables.
/// WarmUpSettings__CronSchedule: "0 */5 * * * *",                              --> warms up request sent in every 5 mins
/// WarmUpSettings__WarmUpRunning: true                                     --> set this to false to stop sending warmup requests
/// WarmUpSettings__Endpoints:0: "https://devpulse-api-1.net/api/health"   --> warm up endpoints of each micro service
/// WarmUpSettings__Endpoints:n: "https://devpulse-api-b.net/api/health"
/// </summary>
public class MicroServiceWarmUpAzureFunction
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly WarmUpSettings _warmUpSettings;

    public MicroServiceWarmUpAzureFunction(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, IOptions<WarmUpSettings> options)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = loggerFactory.CreateLogger<MicroServiceWarmUpAzureFunction>();
        _warmUpSettings = options.Value;
    }

    [Function("MicroServiceWarmUpAzureFunction")]
    public async Task RunAsync([TimerTrigger("%WarmUpSettings__CronSchedule%")] TimerInfo myTimer)
    {
        try
        {
            _logger.LogInformation("MicroServiceWarmUpAzureFunction was executed at: {executionTime}", DateTime.UtcNow);
            if (_warmUpSettings is null)
            {                
                _logger.LogWarning("WarmUpSettings section unavailable: so no warming up!");
                return;
            }

            if (!_warmUpSettings.WarmUpRunning)
            {
                _logger.LogWarning("WarmUpSettings.RunWarmup is set to: {RunWarmup} - so no warming up!", _warmUpSettings.WarmUpRunning);
                return;
            }
            
            if (_warmUpSettings.Endpoints is null || !_warmUpSettings.Endpoints.Any())
            {
                _logger.LogWarning("WarmUpSettings.Endpoints is set to: Empty or null - so no warming up!");
                return;
            }

            await PerformWarmup();

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error occured while executing MicroServiceWarmUpAzureFunction at: {executionTime}", DateTime.Now);
        }
    }

    private async Task PerformWarmup()
    {
        try
        {
            // Thread-Safe Collection to avoid race conditions
            var successStatusCodes = new ConcurrentBag<HttpStatusCode>();

            // Parallel Execution - ensures all warm-up requests run concurrently, not one-by-one.
            await Task.WhenAll(_warmUpSettings.Endpoints.Select(async endpoint => {
                try
                {
                    // using HTTP HEAD to save bandwidth
                    var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, endpoint));
                    successStatusCodes.Add(response.StatusCode);
                    _logger.LogInformation("Warmed up {Endpoint} - Status: {StatusCode}", endpoint, response.StatusCode);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "HTTP request failed while warming up: {Endpoint}", endpoint);
                }
                catch (OperationCanceledException oex)
                {
                    _logger.LogError(oex, "HTTP request was canceled while warming up: {Endpoint}", endpoint);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error in warming up {Endpoint}", endpoint);
                }
            }));

            _logger.LogInformation("Warm-up completed for {WarmedupEndpointsCount} endpoints out of {TotalEndpointsCount}", successStatusCodes.Count, _warmUpSettings.Endpoints.Count);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error occured while executing PerformWarmup at: {executionTime}", DateTime.UtcNow);
            throw;
        }
    }



    /// <summary>
    /// This is for testing purposes 
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Function("ManualWarmUpTriggerAzureFunction")]
    public async Task<HttpResponseData> PerformWarmupTriggerManually([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        _logger.LogInformation("ManualWarmUpTriggerAzureFunction was executed at: {executionTime}", DateTime.UtcNow);
        await PerformWarmup();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Warm-up triggered manually.");
        return response;
    }

}

public class WarmUpSettings
{
    public string CronSchedule { get; set; } = "0 */5 * * * *";                 // Every 5 mins
    public bool WarmUpRunning { get; set; } = true;
    public List<string> Endpoints { get; set; } = [];
}
