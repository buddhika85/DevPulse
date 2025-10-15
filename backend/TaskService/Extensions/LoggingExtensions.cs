using Serilog;

namespace TaskService.Extensions
{
    public static class LoggingExtensions
    {
        public static void AddSerilogLogging(this IHostBuilder hostBuilder)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            hostBuilder.UseSerilog();
        }
    }
}
