using Serilog;
using SharedLib.Configuration.AzureConfig;

namespace MoodService.Extensions
{
    public static class AppInsightsExtensions
    {
        public static IServiceCollection AddAppInsightsTracing(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = configuration
                .GetSection("AzureApplicationInsightsSettings")
                .Get<AzureApplicationInsightsSettings>();

            if (settings is not null && !string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                services.AddApplicationInsightsTelemetry(options =>
                {
                    options.ConnectionString = settings.ConnectionString;
                });

                Log.Information("MoodService API - Azure App Insights Tracing Telemetry initialized");
            }
            else
            {
                Log.Error("MoodService API - Azure App Insights Tracing Telemetry not initialized. Missing connection string.");
            }

            return services;
        }
    }
}
