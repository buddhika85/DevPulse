using Serilog;
using SharedLib.Configuration;

namespace TaskService.Extensions
{
    public static class LoggingExtensions
    {
        public static void AddSerilogLogging(this IHostBuilder hostBuilder, IServiceCollection services, IConfiguration configuration)
        {
            LoggerConfiguration loggerConfiguration = SetupLogConfigurations(services, configuration);

            Log.Logger = loggerConfiguration.CreateLogger();

            hostBuilder.UseSerilog();
        }

        private static LoggerConfiguration SetupLogConfigurations(IServiceCollection services, IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                                                   .MinimumLevel.Information()
                                                   .Enrich.FromLogContext()
                                                   .Enrich.WithMachineName()
                                                   .Enrich.WithThreadId()
                                                   .Enrich.WithProcessId()
                                                   .Enrich.WithEnvironmentUserName()
                                                   .Enrich.WithProperty("Service", "TaskService") // Custom tag
                                                   .WriteTo.Console()
                                                   .WriteTo.File(Path.Combine("Logs", "log_TaskAPI-.txt"),
                                                            retainedFileCountLimit: 2,                              //  Keeps only the latest 2 files
                                                            rollingInterval: RollingInterval.Day);                  // existing file sink --> TaskService / Logs/log_TaskAPI-.txt

            SetupSeqLogVisualizer(services, configuration, loggerConfiguration);

            SetupAzureApplicationInsights(services, configuration, loggerConfiguration);

            SetupAzureBlobStorageForLogs(services, configuration, loggerConfiguration);

            return loggerConfiguration;
        }

        private static void SetupAzureApplicationInsights(IServiceCollection services, IConfiguration configuration, LoggerConfiguration loggerConfiguration)
        {
            services.Configure<AzureApplicationInsightsSettings>(configuration.GetSection("AzureApplicationInsightsSettings"));
            var settings = configuration.GetSection("AzureApplicationInsightsSettings").Get<AzureApplicationInsightsSettings>();
            if (settings is not null && !string.IsNullOrWhiteSpace(settings.ConnectionString))
                loggerConfiguration.WriteTo.ApplicationInsights(
                                    connectionString: settings.ConnectionString,                    // Azure Application Insights Connection String
                                    telemetryConverter: TelemetryConverter.Traces                   // Foward Serilog logs
                                );
        }

        // Writes structured logs to Azure Blob Storage for long-term retention and cost-aware diagnostics
        private static void SetupAzureBlobStorageForLogs(IServiceCollection services, IConfiguration configuration, LoggerConfiguration loggerConfiguration)
        {
            services.Configure<AzureBlobStorageSettings>(configuration.GetSection("AzureBlobStorageSettings"));
            var settings = configuration.GetSection("AzureBlobStorageSettings").Get<AzureBlobStorageSettings>();
            if (settings is not null && !string.IsNullOrWhiteSpace(settings.ConnectionString) && !string.IsNullOrWhiteSpace(settings.LogsContainerName))
            {
                var dateSuffix = DateTime.UtcNow.ToString("yyyy-MM-dd");
                var blobFileName = $"log_TaskAPI-{dateSuffix}.txt";                 // creates a new blob file per day, similar to rolling file behavior.
                loggerConfiguration.WriteTo.AzureBlobStorage(
                        connectionString: settings.ConnectionString,
                        storageContainerName: settings.LogsContainerName,
                        storageFileName: blobFileName,                              // log_TaskAPI-2025-11-18.txt
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}");
            }            
        }

        private static void SetupSeqLogVisualizer(IServiceCollection services, IConfiguration configuration, LoggerConfiguration loggerConfiguration)
        {
            // Maps the "SeqLogVisualizerSettings" section from appsettings.json to the strongly typed SeqLogVisualizerSettings class.          
            services.Configure<SeqLogVisualizerSettings>(configuration.GetSection("SeqLogVisualizerSettings"));

            // Retrieves the bound SeqLogVisualizerSettings object directly from configuration.            
            var seqLogVisualizerSettings = configuration.GetSection("SeqLogVisualizerSettings").Get<SeqLogVisualizerSettings>();

            // Defensive check: ensures the section exists and is properly bound.
            // If null: not log messages not written to Seq Web View
            if (seqLogVisualizerSettings is not null && !string.IsNullOrWhiteSpace(seqLogVisualizerSettings.Url))
                loggerConfiguration.WriteTo.Seq(seqLogVisualizerSettings.Url);                                               // default Seq Port 5341
        }
    }
}
