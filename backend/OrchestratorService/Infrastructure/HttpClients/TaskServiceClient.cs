using SharedLib.DTOs.Task;


namespace OrchestratorService.Infrastructure.HttpClients
{
    public class TaskServiceClient : ITaskServiceClient
    {
        private readonly HttpClient _httpClient;

        public TaskServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TaskItemDto>> GetTasksAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var userIdGuid))
                throw new ArgumentException("Invalid user ID format");

            // call user API to get tasks by user Id
            var tasks = await _httpClient.GetFromJsonAsync<List<TaskItemDto>>($"api/tasks/by-user/{userIdGuid}");

            return tasks
                ?? [];
        }
    }

}
