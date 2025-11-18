using Serilog;
using Serilog.Configuration;

namespace SharedLib.Logging
{
    /// <summary>
    /// Extension method to register the custom daily rolling blob sink with Serilog.
    /// </summary>
    public static class DailyRollingBlobSinkExtensions
    {
        public static LoggerConfiguration AzureDailyBlob(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string connectionString,
            string containerName,
            string prefix = "log_",
            string suffix = ".txt",
            IFormatProvider? formatProvider = null)
        {
            return loggerSinkConfiguration.Sink(
                new DailyRollingBlobSink(connectionString, containerName, prefix, suffix, formatProvider)
            );
        }
    }
}