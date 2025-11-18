using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DevPulseOrchestratorFn.Extensions; 

// Create the host builder for the Azure Function App
var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWebApplication() // Required for isolated worker model
    .ConfigureAppConfiguration(config =>
    {
        // Load configuration from JSON files (local + app settings)
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
    });

// ✅ Build a temporary configuration to pass into logging setup
var tempConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .Build();

// ✅ Create a temporary service collection for logging setup
var tempServices = new ServiceCollection();

// ✅ Inject Serilog logging (writes to App Insights, file, Seq)
hostBuilder.AddSerilogLogging(tempServices, tempConfig);

// Register services and telemetry
hostBuilder.ConfigureServices((context, services) =>
{
    services.AddHttpClient(); // ✅ For outbound HTTP calls
    services.AddApplicationInsightsTelemetryWorkerService(); // ✅ Enables App Insights telemetry
    services.ConfigureFunctionsApplicationInsights();         // ✅ Binds telemetry to Functions runtime
});

// ✅ Build and run the host
var host = hostBuilder.Build();
host.Run();