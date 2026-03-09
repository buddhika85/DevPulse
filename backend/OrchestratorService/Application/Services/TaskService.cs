using Microsoft.Extensions.Caching.Memory;
using OrchestratorService.Infrastructure.HttpClients.TaskMicroService;
using OrchestratorService.Infrastructure.HttpClients.UserMicroService;
using SharedLib.DTOs.Task;
using SharedLib.DTOs.User;

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

        public async Task<IReadOnlyList<TaskItemWithUserDto>> GetTasksByTeam(Guid managerId, bool includeDeleted,  CancellationToken cancellationToken)
        {
           
            _logger.LogInformation("Finding tasks for team by manager {ManagerId} (includeDeleted={IncludeDeleted}) at {Time}", managerId, includeDeleted, DateTime.UtcNow);

            var cacheKey = $"TeamMembersForManagerId:{managerId}";

            try
            {
                _logger.LogInformation("Step 1 - retrieving team members for manager {ManagerId}", managerId);

                if (!_inMemoryCache.TryGetValue(cacheKey, out IReadOnlyList<UserAccountDto>? teamMembers) || teamMembers is null)
                {
                    _logger.LogInformation("Cache miss for manager {ManagerId}. Requesting from User Microservice.", managerId);
                    teamMembers = await _userClient.GetTeamMembersForManager(managerId, cancellationToken);

                    if (teamMembers is not null)
                    {
                        _logger.LogInformation("Caching {TeamMemberCount} team members for manager {ManagerId}", teamMembers.Count, managerId);
                        _inMemoryCache.Set(cacheKey, teamMembers, TimeSpan.FromMinutes(CachedTimeInMins));
                    }
                }
                else
                {
                    _logger.LogInformation("Cache hit for manager {ManagerId}", managerId);
                }

                if (teamMembers is null || !teamMembers.Any())
                {
                    _logger.LogInformation("No team members found for manager {ManagerId}", managerId);
                    return [];
                }

                _logger.LogInformation("Step 2 - retrieving tasks for {TeamMemberCount} team members", teamMembers.Count);

                var tasks = await _taskClient.GetTasksForTeamMembers([.. teamMembers.Select(x => x.Id)], includeDeleted, cancellationToken);



                _logger.LogInformation("Step 3 - Hydrating task items with assigned user display name");

                // enrich tasks with assigned user display name
                var tasksEnrichedWithUser = HydrateTaskWithUserDisplayName(teamMembers, tasks);

                return tasksEnrichedWithUser.AsReadOnly();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve team tasks for manager {ManagerId}", managerId);
                throw;
            }

        }

        /// <summary>
        /// Enriches a collection of task items with the display name of the user
        /// they are assigned to. This method performs a left join between the
        /// provided task list and the manager's team member list, ensuring that
        /// tasks are still returned even if the assigned user is no longer part
        /// of the team.
        /// </summary>
        /// <param name="teamMembers">
        /// The list of users belonging to the manager's team. Used to resolve
        /// the display name for each task's assigned user.
        /// </param>
        /// <param name="tasks">
        /// The list of task items retrieved from the Task microservice. Each task
        /// contains a UserId that is matched against the team member list.
        /// </param>
        /// <returns>
        /// A list of <see cref="TaskItemWithUserDto"/> objects where each task is
        /// enriched with the corresponding user's display name. If a matching user
        /// cannot be found (e.g., user removed from team), the display name is set
        /// to "unknown".
        /// </returns>
        private static List<TaskItemWithUserDto> HydrateTaskWithUserDisplayName(IReadOnlyList<UserAccountDto> teamMembers, IReadOnlyList<TaskItemDto> tasks)
        {
            return [.. (from task in tasks
                    join user in teamMembers on task.UserId equals user.Id into userGroup
                    from user in userGroup.DefaultIfEmpty()
                    orderby task.CreatedAt descending
                    select new TaskItemWithUserDto
                    {
                        Id = task.Id,
                        UserId = task.UserId,
                        CreatedAt = task.CreatedAt,
                        Description = task.Description,
                        DueDate = task.DueDate,
                        IsDeleted = task.IsDeleted,
                        Priority = task.Priority,
                        Status = task.Status,
                        Title = task.Title,
                        UserDisplayName = user.DisplayName ?? "unknown"
                    })];
        }
    }
}
