using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SharedLib.UnitTesting;
using TaskJournalLinkService.Repositories;
using TaskJournalLinkService.Services;

namespace TaskJournalLinkService.Tests.Services
{
    public class TaskJournalLinkServiceTests
    {
        private readonly Mock<ITaskJournalLinkRepository> _mockRepository;
        private readonly Mock<ILogger<TaskJournalLinkService.Services.TaskJournalLinkService>> _mockLogger;

        private readonly ITaskJournalLinkService _cut;      // class Under test

        public TaskJournalLinkServiceTests()
        {
            _mockRepository = new Mock<ITaskJournalLinkRepository>();
            _mockLogger = new Mock<ILogger<TaskJournalLinkService.Services.TaskJournalLinkService>>();

            _cut = new TaskJournalLinkService.Services.TaskJournalLinkService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetLinksByJournalIdAsync_ReturnsLinksArray_WhenLinksAvailable()
        {
            // arrange
            var journalId = Guid.NewGuid();
            var testData = new Domain.Models.TaskJournalLinkDocument[]
            {
                new(Guid.NewGuid(), Guid.NewGuid(), journalId.ToString(), DateTime.Today),
                new(Guid.NewGuid(), Guid.NewGuid(), journalId.ToString(), DateTime.Today)
            };
            _mockRepository
                .Setup(x => x.GetLinksByJournalIdAsync(journalId, CancellationToken.None))
                .ReturnsAsync(testData);

            // act
            var taskJournalLinks = await _cut.GetLinksByJournalIdAsync(journalId, CancellationToken.None);

            // assert
            taskJournalLinks.Should().NotBeNull();
            taskJournalLinks.Should().HaveCount(testData.Length);

            // asserting on log messages
            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Retrieving TaskJournalLinks for JournalId {journalId}",
                Times.Once());

            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Retrieved {testData.Length} TaskJournalLink documents for JournalId {journalId}",
                Times.Once());

            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Mapped {testData.Length} TaskJournalLink DTOs for JournalId {journalId}",
                Times.Once());


            _mockLogger.VerifyMessage(LogLevel.Error,
                $"Error querying TaskJournalLinks for JournalId {journalId}",
                Times.Never());


            // verify repository GetLinksByJournalIdAsync was called once
            _mockRepository.Verify(x => x.GetLinksByJournalIdAsync(journalId, It.IsAny<CancellationToken>()),
                Times.Once());
        }

        // Test: Returns empty array when no links exist
        [Fact]
        public async Task GetLinksByJournalIdAsync_ReturnsEmptyLinksArray_WhenLinksUnavailable()
        {
            // arrange
            var journalId = Guid.NewGuid();
            _mockRepository
                .Setup(x => x.GetLinksByJournalIdAsync(journalId, CancellationToken.None))
                .ReturnsAsync(Array.Empty<Domain.Models.TaskJournalLinkDocument>());

            // act
            var taskJournalLinks = await _cut.GetLinksByJournalIdAsync(journalId, CancellationToken.None);

            // assert
            taskJournalLinks.Should().NotBeNull();
            taskJournalLinks.Should().BeEmpty();

            // asserting on log messages
            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Retrieving TaskJournalLinks for JournalId {journalId}",
                Times.Once());

            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Retrieved 0 TaskJournalLink documents for JournalId {journalId}",
                Times.Once());

            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Mapped 0 TaskJournalLink DTOs for JournalId {journalId}",
                Times.Once());


            _mockLogger.VerifyMessage(LogLevel.Error,
                $"Error querying TaskJournalLinks for JournalId {journalId}",
                Times.Never());


            // verify repository GetLinksByJournalIdAsync was called once
            _mockRepository.Verify(x => x.GetLinksByJournalIdAsync(journalId, It.IsAny<CancellationToken>()),
                 Times.Once());
        }

        // Test: Logs error when repository throws
        [Fact]
        public async Task GetLinksByJournalIdAsync_ThrowsAndLogsError_WhenRepositoryThrows()
        {
            // arrange
            var journalId = Guid.NewGuid();
            var exception = new Exception("DB failure");

            _mockRepository
                .Setup(x => x.GetLinksByJournalIdAsync(journalId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // act
            Func<Task> act = async () =>
                await _cut.GetLinksByJournalIdAsync(journalId, CancellationToken.None);

            // assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("DB failure");

            // verify error log
            _mockLogger.VerifyMessage(
                LogLevel.Error,
                $"Error retrieving TaskJournalLinks for JournalId {journalId}",
                Times.Once());

            // verify success logs NEVER happened
            _mockLogger.VerifyMessage(
                LogLevel.Information,
                $"Retrieved",
                Times.Never());

            _mockLogger.VerifyMessage(
                LogLevel.Information,
                $"Mapped",
                Times.Never());

            // verify repository was called once
            _mockRepository.Verify(
                x => x.GetLinksByJournalIdAsync(journalId, It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}
