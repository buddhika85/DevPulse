using Microsoft.Extensions.Caching.Memory;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Infrastructure.HttpClients.TaskMicroService;
using OrchestratorService.Infrastructure.HttpClients.UserMicroService;
using SharedLib.DTOs.Task;

namespace OrchestratorService.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUserServiceClient _userClient;
        private readonly ITaskServiceClient _taskClient;
        private readonly IMemoryCache _inMemoryCache;
        private readonly ILogger<TaskService> _logger;
        private const byte CachedTimeInMins = 5;                // Cache time in minutes

        public TaskService(IUserServiceClient userClient, ITaskServiceClient taskClient, IMemoryCache cache, ILogger<TaskService> logger)
        {
            _userClient = userClient;
            _taskClient = taskClient;
            _inMemoryCache = cache;
            _logger = logger;
        }

        public async Task<IReadOnlyList<TaskItemDto>> GetTasksByTeam(Guid managerId, bool includeDeleted,  CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Finding tasks for team by manager {ManagerId} (includeDeleted={IncludeDeleted}) at {Time}", managerId, includeDeleted, now);

            var cacheKey = $"TeamMembersForManagerId:{managerId}";

            try
            {
                _logger.LogInformation("Step 1 - retrieving team member IDs for manager {ManagerId}", managerId);

                if (!_inMemoryCache.TryGetValue(cacheKey, out IReadOnlyList<Guid>? teamMemberIds) || teamMemberIds is null)
                {
                    _logger.LogInformation("Cache miss for manager {ManagerId}. Requesting from User Microservice.", managerId);
                    teamMemberIds = await _userClient.GetTeamMembersForManager(managerId, cancellationToken);

                    if (teamMemberIds is not null)
                    {
                        _logger.LogInformation("Caching {TeamMemberCount} team members for manager {ManagerId}", teamMemberIds.Count, managerId);
                        _inMemoryCache.Set(cacheKey, teamMemberIds, TimeSpan.FromMinutes(CachedTimeInMins));
                    }
                }
                else
                {
                    _logger.LogInformation("Cache hit for manager {ManagerId}", managerId);
                }

                if (teamMemberIds is null || !teamMemberIds.Any())
                {
                    _logger.LogInformation("No team members found for manager {ManagerId}", managerId);
                    return [];
                }

                _logger.LogInformation("Step 2 - retrieving tasks for {TeamMemberCount} team members", teamMemberIds.Count);

                var tasks = await _taskClient.GetTasksForTeamMembers(teamMemberIds, includeDeleted, cancellationToken);

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve team tasks for manager {ManagerId}", managerId);
                throw;
            }

        }
    }
}
