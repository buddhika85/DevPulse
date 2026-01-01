using SharedLib.DTOs.Task;
using System.Text.Json;


namespace OrchestratorService.Infrastructure.HttpClients.TaskMicroService
{
    public class TaskServiceClient : ITaskServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TaskServiceClient> _logger;

        private const string routeTasksByUserId = "api/tasks/by-user/";

        public TaskServiceClient(HttpClient httpClient, ILogger<TaskServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<TaskItemDto>> GetTasksAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var userIdGuid))
            {
                _logger.LogWarning("Invalid user ID format received: {UserId}", userId);
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }

            try
            {
                _logger.LogInformation("Fetching tasks for user {UserId}", userIdGuid);

                var tasks = await _httpClient.GetFromJsonAsync<List<TaskItemDto>>(
                    $"{routeTasksByUserId}{userIdGuid}", cancellationToken);            // api/tasks/by-user/user-id

                if (tasks == null || tasks.Count == 0)
                {
                    _logger.LogInformation("No tasks found for user {UserId}", userIdGuid);
                    return [];
                }

                _logger.LogInformation("Retrieved {TaskCount} tasks for user {UserId}", tasks.Count, userIdGuid);
                return tasks;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching tasks for user {UserId}", userIdGuid);
                throw new ApplicationException("Failed to fetch tasks from user API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for tasks response for user {UserId}", userIdGuid);
                throw new ApplicationException("Failed to deserialize task data", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Task fetching operation was canceled for user {UserId}", userIdGuid);
                throw;
            }
        }

    }

}
