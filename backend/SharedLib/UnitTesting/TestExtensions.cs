using Microsoft.Extensions.Logging;
using Moq;

namespace SharedLib.UnitTesting
{
    public static class TestExtensions
    {
        /// <summary>
        /// Verifies that a specific log message was emitted with the expected log level and frequency.
        /// </summary>
        /// <typeparam name="T">The type associated with the generic ILogger being mocked.</typeparam>
        /// <param name="mockLogger">The mocked ILogger instance to verify against.</param>
        /// <param name="level">The LogLevel to assert (e.g., Information, Warning).</param>
        /// <param name="expectedLogMessage">The expected substring within the log message.</param>
        /// <param name="time">The Moq.Times constraint (e.g., Times.Once()).</param>
        public static void VerifyMessage<T>(
           this Mock<ILogger<T>> mockLogger,
           LogLevel level,
           string expectedLogMessage, Times time)
        {
            mockLogger.Verify(x =>
                x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object value, Type t) => Helpers.CheckForLogMessage(value, expectedLogMessage)),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                time);
        }

    }


    public static class Helpers
    {
        public static bool CheckForLogMessage(object value, string expectedLogMessage)
        {
            return value?.ToString() is string message
                && message.Contains(expectedLogMessage);
        }
    }
}
