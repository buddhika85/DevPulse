using Serilog;
using SharedLib.Configuration.logging;

namespace MoodService.Extensions
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
                                                   .Enrich.WithProperty("Service", "MoodService") // Custom tag
                                                   .WriteTo.Console()
                                                   .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);                // UserService/Logs/Log-.txt


            // Maps the "SeqLogVisualizerSettings" section from appsettings.json to the strongly typed SeqLogVisualizerSettings class.          
            services.Configure<SeqLogVisualizerSettings>(configuration.GetSection("SeqLogVisualizerSettings"));

            // Retrieves the bound SeqLogVisualizerSettings object directly from configuration.            
            var seqLogVisualizerSettings = configuration.GetSection("SeqLogVisualizerSettings").Get<SeqLogVisualizerSettings>();

            // Defensive check: ensures the section exists and is properly bound.
            // If null: not log messages not written to Seq Web View
            if (seqLogVisualizerSettings is not null && !string.IsNullOrWhiteSpace(seqLogVisualizerSettings.Url))
                loggerConfiguration.WriteTo.Seq(seqLogVisualizerSettings.Url);                                               // default Seq Port 5341
            return loggerConfiguration;
        }
    }
}
