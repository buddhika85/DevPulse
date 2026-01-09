using SharedLib.DTOs.Journal;
using SharedLib.DTOs.TaskJournalLink;
using System.Text.Json;

namespace OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService
{
    public class TaskJournalLinkService : ITaskJournalLinkService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TaskJournalLinkService> _logger;

        private const string TaskJournalLinksRoute = "api/taskJournalLinks/";

        public TaskJournalLinkService(HttpClient httpClient, ILogger<TaskJournalLinkService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }



        public async Task<TaskJournalLinkDto[]> LinkNewJournalWithTasks(Guid journalId, Guid[] linkedTaskIds, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Linking {TaskCount} tasks to journal {JournalId}",
                    linkedTaskIds.Length, journalId);

                var payload = new LinkTasksToJournalDto
                {
                    JournalId = journalId,
                    TaskIdsToLink = linkedTaskIds
                };

                var response = await _httpClient.PostAsJsonAsync(
                    TaskJournalLinksRoute,
                    payload,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogWarning(
                        "TaskJournalLink API returned {StatusCode}. Body: {Body}",
                        response.StatusCode,
                        errorBody);

                    throw new ApplicationException(
                        $"TaskJournalLink API failed with status code {response.StatusCode}");
                }

                var links = await response.Content.ReadFromJsonAsync<TaskJournalLinkDto[]>(
                    cancellationToken: cancellationToken);

                if (links is null)
                {
                    _logger.LogWarning(
                        "TaskJournalLink API returned null for journal {JournalId}",
                        journalId);

                    throw new ApplicationException(
                        "TaskJournalLink API returned null instead of TaskJournalLinkDto[]");
                }

                _logger.LogInformation(
                    "Successfully created {Count} TaskJournalLinks for journal {JournalId}",
                    links.Length,
                    journalId);

                return links;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "HTTP request failed while linking tasks for journal {JournalId}",
                    journalId);

                throw new ApplicationException(
                    "Failed to call TaskJournalLink API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Deserialization failed for TaskJournalLink API response for journal {JournalId}",
                    journalId);

                throw new ApplicationException(
                    "Failed to deserialize TaskJournalLink API response", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "TaskJournalLink operation was canceled for journal {JournalId}",
                    journalId);

                throw;
            }
        }
    }
}
