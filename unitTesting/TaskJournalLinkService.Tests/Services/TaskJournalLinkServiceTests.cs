using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SharedLib.UnitTesting;
using TaskJournalLinkService.Mapper;
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

        #region GetLinksByJournalIdAsync

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

        #endregion GetLinksByJournalIdAsync

        [Fact]
        public async Task GetLinksForJournalIdsAsync_ShouldReturnMappedDtos_WhenRepositoryReturnsEntities()
        {
            // Arrange
            IReadOnlyList<Guid> journalIds = [Guid.NewGuid(), Guid.NewGuid()];
            var testDate = new DateTime(2024, 01, 01);
            var repositoryEntities = new Domain.Models.TaskJournalLinkDocument[]
            {
                new(Guid.NewGuid(), Guid.NewGuid(), journalIds[0].ToString(), testDate),
                new(Guid.NewGuid(), Guid.NewGuid(), journalIds[0].ToString(), testDate),
                new(Guid.NewGuid(), Guid.NewGuid(), journalIds[1].ToString(), testDate)
            };
            _mockRepository
                .Setup(x => x.GetLinksByJournalIdAsync(journalIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(repositoryEntities);
            var expectedDtos = TaskJournalLinkMapper.ToDtos(repositoryEntities).ToList();
            var journalIdsStr = string.Join(",", journalIds);

            // Act
            var result = await _cut.GetLinksForJournalIdsAsync(journalIds, CancellationToken.None);

            // Assert - Result
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(expectedDtos.Count);
            result.Should().BeEquivalentTo(expectedDtos);


            // Assert - Logging
            _mockLogger.VerifyMessage(LogLevel.Warning,
                "Empty journalIds list provided",
                Times.Never());

            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Retrieving TaskJournalLinks for JournalIds {journalIdsStr}",
                Times.Once());

            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Retrieved {repositoryEntities.Length} TaskJournalLink documents for JournalIds {journalIdsStr}",
                Times.Once());

            _mockLogger.VerifyMessage(LogLevel.Information,
                $"Mapped {expectedDtos.Count} TaskJournalLink DTOs for JournalIds {journalIdsStr}",
                Times.Once());


            _mockLogger.VerifyMessage(LogLevel.Error,
                $"Error retrieving TaskJournalLinks for JournalIds {journalIdsStr}",
                Times.Never());


            // Assert - Repository call
            _mockRepository.Verify(x => x.GetLinksByJournalIdAsync(journalIds, It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }


    // empty list, exception path, null input
}
