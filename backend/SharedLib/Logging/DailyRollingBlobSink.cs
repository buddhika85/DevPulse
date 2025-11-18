using Azure.Storage.Blobs;
using Serilog.Core;
using Serilog.Events;
using System.Globalization;

namespace SharedLib.Logging
{
    /// <summary>
    /// Custom Serilog sink that writes log events to Azure Blob Storage,
    /// creating a new blob file each UTC day.
    /// </summary>
    public class DailyRollingBlobSink : ILogEventSink, IDisposable
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _prefix;
        private readonly string _suffix;
        private readonly IFormatProvider _formatProvider;
        private StreamWriter? _writer;
        private DateTime _currentDate;

        public DailyRollingBlobSink(string connectionString, string containerName, string prefix, string suffix, IFormatProvider? formatProvider = null)
        {
            _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
            _prefix = prefix;
            _suffix = suffix;
            _currentDate = DateTime.UtcNow.Date;

            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists(); 

            InitializeWriter();
        }

        /// <summary>
        /// Creates or switches to the blob file for the current UTC day.
        /// </summary>

        private void InitializeWriter()
        {
            var blobName = $"{_prefix}{_currentDate:yyyy-MM-dd}{_suffix}";
            var blobClient = _containerClient.GetBlobClient(blobName);
            var stream = blobClient.OpenWrite(overwrite: true);
            _writer = new StreamWriter(stream) { AutoFlush = true };
        }

        /// <summary>
        /// Writes a log event to the current day's blob file.
        /// Rolls over if the UTC date changes.
        /// </summary>

        public void Emit(LogEvent logEvent)
        {
            var now = DateTime.UtcNow.Date;
            if (now > _currentDate)
            {
                _writer?.Dispose();
                _currentDate = now;
                InitializeWriter();
            }

            var message = logEvent.RenderMessage(_formatProvider);
            _writer?.WriteLine($"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss} [{logEvent.Level}] {message}");
            if (logEvent.Exception != null)
            {
                _writer?.WriteLine(logEvent.Exception);
            }
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }
    }
}