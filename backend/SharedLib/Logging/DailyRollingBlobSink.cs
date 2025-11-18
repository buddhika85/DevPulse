using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Serilog.Core;
using Serilog.Events;
using System.Globalization;
using System.Text;

namespace SharedLib.Logging
{
    /// <summary>
    /// Custom Serilog sink that writes log events to Azure Blob Storage.
    /// It creates a new .txt file each day based on Sydney time.
    /// </summary>
    public class DailyRollingBlobSink : ILogEventSink
    {
        // Azure Blob container client
        private readonly BlobContainerClient _containerClient;

        // Filename prefix and suffix (e.g., "log_TaskAPI-" + "2025-11-19" + ".txt")
        private readonly string _prefix;
        private readonly string _suffix;

        // Controls how log messages are formatted
        private readonly IFormatProvider _formatProvider;

        // Tracks the current Sydney calendar day
        private DateTime _currentDate;

        /// <summary>
        /// Initializes the sink with connection info and sets the current date.
        /// </summary>
        public DailyRollingBlobSink(string connectionString, string containerName, string prefix, string suffix, IFormatProvider? formatProvider = null)
        {
            _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
            _prefix = prefix;
            _suffix = suffix;

            // Get current date in Sydney time zone
            _currentDate = GetSydneyDate();

            // Connect to the blob container and create it if missing
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists();
        }

        /// <summary>
        /// Called by Serilog for each log event.
        /// Appends the log line to the correct daily blob file.
        /// </summary>
        public void Emit(LogEvent logEvent)
        {
            // Check if the day has changed (Sydney time)
            var now = GetSydneyDate();
            if (now > _currentDate)
            {
                _currentDate = now;
            }

            // Format the log message
            var message = logEvent.RenderMessage(_formatProvider);
            var fullLine = $"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss} [{logEvent.Level}] {message}";

            // Include exception details if present
            if (logEvent.Exception != null)
            {
                fullLine += Environment.NewLine + logEvent.Exception;
            }

            // Write the log line to blob
            EmitLogLine(fullLine);
        }

        /// <summary>
        /// Appends a single log line to the current day's blob file.
        /// </summary>
        private void EmitLogLine(string line)
        {
            // Build the blob filename using prefix + date + suffix
            var blobName = $"{_prefix}{_currentDate:yyyy-MM-dd}{_suffix}";
            var appendBlobClient = _containerClient.GetAppendBlobClient(blobName);

            // Create the blob if it doesn't exist yet
            try
            {
                appendBlobClient.CreateIfNotExists();
            }
            catch
            {
                // Optional: handle transient errors silently
            }

            // Convert the log line to a stream
            var bytes = Encoding.UTF8.GetBytes(line + Environment.NewLine);
            using var stream = new MemoryStream(bytes);

            // Append the stream to the blob
            appendBlobClient.AppendBlock(stream);
        }

        /// <summary>
        /// Returns the current date in Sydney time zone.
        /// </summary>
        private static DateTime GetSydneyDate()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time")).Date;
        }
    }
}