using SharedLib.DTOs.Journal;
using SharedLib.DTOs.TaskJournalLink;
using System.Net;
using System.Text.Json;

namespace OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService
{
    public class TaskJournalLinkServiceClient : ITaskJournalLinkServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TaskJournalLinkServiceClient> _logger;

        private const string TaskJournalLinksRoute = "api/TaskJournalLinks";

        public TaskJournalLinkServiceClient(HttpClient httpClient, ILogger<TaskJournalLinkServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }



        public async Task<TaskJournalLinkDto[]> LinkNewJournalWithTasks(Guid journalId, HashSet<Guid> linkedTaskIds, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Linking {TaskCount} tasks to journal {JournalId}",
                    linkedTaskIds.Count, journalId);

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

        public async Task<TaskJournalLinkDto[]> GetLinksByJournalIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching task journal links by journal ID: {JournalId}", id);

                var url = $"{TaskJournalLinksRoute}/{id}";
                _logger.LogDebug("Calling GET {Url}", url);
                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("No Tasks linked to Journal Id: {JournalId}", id);
                    return [];
                }

                response.EnsureSuccessStatusCode();     // throws exception if its not a success code

                var links = await response.Content.ReadFromJsonAsync<TaskJournalLinkDto[]>(cancellationToken: cancellationToken);
                if (links == null)
                {
                    _logger.LogInformation("No Tasks linked to Journal Id: {JournalId}", id);
                    return [];
                }
                return links;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching links by journal ID: {JournalId}", id);
                throw new ApplicationException("Failed to fetch tasks from user API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for TaskJournalLinks response for journal Id {JournalId}", id);
                throw new ApplicationException("Failed to deserialize TaskJournalLinks data", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("TaskJournalLinks fetching operation was canceled for journal Id {JournalId}", id);
                throw;
            }
        }

        public async Task<bool> RearrangeTaskJournalLinks(Guid journalId, HashSet<Guid> linkedTaskIds, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var links = $"[{string.Join(",", linkedTaskIds)}]";

            _logger.LogInformation(
                "Calling TaskJournalLink API to rearrange links for JournalId={JournalId} with TaskLinks={TaskLinks} at {Time}",
                journalId, links, now);

            try
            {
                var url = $"{TaskJournalLinksRoute}/rearrange-links/{journalId}";
                var response = await _httpClient.PutAsJsonAsync(url, linkedTaskIds, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogWarning(
                        "TaskJournalLink API returned failure for JournalId={JournalId}. StatusCode={StatusCode}, Body={Body}",
                        journalId, response.StatusCode, errorBody);

                    throw new ApplicationException(
                        $"TaskJournalLink API failed with status code {response.StatusCode} for JournalId={journalId}");
                }

                _logger.LogInformation(
                    "Successfully rearranged TaskJournalLinks for JournalId={JournalId} at {Time}",
                    journalId, now);

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "HTTP request error while calling TaskJournalLink API for JournalId={JournalId}",
                    journalId);

                throw new ApplicationException(
                    $"Failed to call TaskJournalLink API for JournalId={journalId}", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "JSON serialization/deserialization error while calling TaskJournalLink API for JournalId={JournalId}",
                    journalId);

                throw new ApplicationException(
                    $"Failed to process TaskJournalLink API response for JournalId={journalId}", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "TaskJournalLink API call was canceled for JournalId={JournalId}",
                    journalId);

                throw;
            }
        }
    }
}
