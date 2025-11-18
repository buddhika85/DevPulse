using Serilog;
using SharedLib.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevPulseOrchestratorFn.Extensions
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
                                                   .Enrich.FromLogContext()
                                                   .Enrich.WithMachineName()
                                                   .Enrich.WithThreadId()
                                                   .Enrich.WithProcessId()
                                                   .Enrich.WithEnvironmentUserName()
                                                   .Enrich.WithProperty("Service", "AzureFunction-DevPulseOrchestratorFn") // Custom tag
                                                   .WriteTo.Console()
                                                   .WriteTo.File("Logs/log_AzFuncApp-.txt",
                                                        retainedFileCountLimit: 2,                              //  Keeps only the latest 2 files
                                                        rollingInterval: RollingInterval.Day);                  // DevPulseOrchestratorFn/Logs/log_AzFuncApp-.txt


            SetupSeqLogVisualizer(services, configuration, loggerConfiguration);

            SetupAzureApplicationInsights(services, configuration, loggerConfiguration);

            return loggerConfiguration;
        }

        // Reads from Loal.Settings.json
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

        // Reads from appSettings.json
        private static void SetupSeqLogVisualizer(IServiceCollection services, IConfiguration configuration, LoggerConfiguration loggerConfiguration)
        {
            // Maps the "SeqLogVisualizerSettings" section from appsettings.json to the strongly typed SeqLogVisualizerSettings class.          
            services.Configure<SeqLogVisualizerSettings>(configuration.GetSection("SeqLogVisualizerSettings"));

            // Retrieves the bound SeqLogVisualizerSettings object directly from configuration.            
            var seqLogVisualizerSettings = configuration.GetSection("SeqLogVisualizerSettings").Get<SeqLogVisualizerSettings>();

            // Defensive check: ensures the section exists and is properly bound.
            // If null: not log messages not written to Seq Web View
            if (seqLogVisualizerSettings is not null && !string.IsNullOrWhiteSpace(seqLogVisualizerSettings.Url))
                loggerConfiguration.WriteTo.Seq(seqLogVisualizerSettings.Url);                                               // Seq Port 5344
        }
    }
}
