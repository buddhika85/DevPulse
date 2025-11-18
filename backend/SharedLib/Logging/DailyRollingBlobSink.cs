using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Serilog.Core;
using Serilog.Events;
using System.Globalization;
using System.Text;

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
            var appendBlobClient = _containerClient.GetAppendBlobClient(blobName);

            // Create the blob if it doesn't exist
            appendBlobClient.CreateIfNotExists();

            // No stream writer needed — appending will happen in Emit()
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
                _currentDate = now;
            }

            var message = logEvent.RenderMessage(_formatProvider);
            var fullLine = $"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss} [{logEvent.Level}] {message}";
            if (logEvent.Exception != null)
            {
                fullLine += Environment.NewLine + logEvent.Exception;
            }

            EmitLogLine(fullLine);
        }

        private void EmitLogLine(string line)
        {
            var blobName = $"{_prefix}{_currentDate:yyyy-MM-dd}{_suffix}";
            var appendBlobClient = _containerClient.GetAppendBlobClient(blobName);

            // Create the blob if it doesn't exist
            appendBlobClient.CreateIfNotExists();

            // Convert the line to a stream
            var bytes = Encoding.UTF8.GetBytes(line + Environment.NewLine);
            using var stream = new MemoryStream(bytes);

            // Append the stream to the blob
            appendBlobClient.AppendBlock(stream);
        }


        public void Dispose()
        {
            _writer?.Dispose();
        }
    }
}