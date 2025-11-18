using Serilog;
using SharedLib.Configuration;
using SharedLib.Logging;

namespace UserService.Extensions
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
                                                   .Enrich.WithProperty("Service", "UserService") // Custom tag
                                                   .WriteTo.Console()
                                                   .WriteTo.File(Path.Combine("Logs", "log_UserAPI-.txt"),
                                                        retainedFileCountLimit: 2,                              //  Keeps only the latest 2 files
                                                        rollingInterval: RollingInterval.Day);                  // UserService/Logs/log_UserAPI-.txt


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

        /// <summary>
        /// Configures Serilog to write structured logs to Azure Blob Storage,
        /// creating a new .txt file each UTC day for long-term retention and cost-aware diagnostics.
        /// </summary>
        private static void SetupAzureBlobStorageForLogs(
            IServiceCollection services,
            IConfiguration configuration,
            LoggerConfiguration loggerConfiguration)
        {
            // Bind AzureBlobStorageSettings from appsettings.json and local.settings.json
            services.Configure<AzureBlobStorageSettings>(configuration.GetSection("AzureBlobStorageSettings"));
            var blobSettings = configuration.GetSection("AzureBlobStorageSettings").Get<AzureBlobStorageSettings>();

            // Validate that required settings are present
            if (blobSettings is not null &&
                !string.IsNullOrWhiteSpace(blobSettings.ConnectionString) &&
                !string.IsNullOrWhiteSpace(blobSettings.LogsContainerName))
            {
                // Register the custom daily rolling sink from SharedLib.Logging
                loggerConfiguration.WriteTo.AzureDailyBlob(
                    connectionString: blobSettings.ConnectionString,
                    containerName: blobSettings.LogsContainerName,
                    prefix: "log_UserAPI-",
                    suffix: ".txt"
                );
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
                loggerConfiguration.WriteTo.Seq(seqLogVisualizerSettings.Url);                                               // Seq Port 5342
        }
    }



}
