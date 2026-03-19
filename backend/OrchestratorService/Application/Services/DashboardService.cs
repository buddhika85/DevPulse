using Microsoft.Extensions.Caching.Memory;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Infrastructure.HttpClients.JournalMicroService;
using OrchestratorService.Infrastructure.HttpClients.TaskMicroService;
using OrchestratorService.Infrastructure.HttpClients.UserMicroService;
using SharedLib.DTOs.Journal;
using SharedLib.DTOs.ManagerDashboard;
using SharedLib.DTOs.Task;
using SharedLib.DTOs.User;
using SharedLib.Helpers;
using System.Globalization;
using ManagerDashboardDto = SharedLib.DTOs.ManagerDashboard.ManagerDashboardDto;

namespace OrchestratorService.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserServiceClient _userClient;
        private readonly ITaskServiceClient _taskClient;
        private readonly IJournalServiceClient _journalClient;
        private readonly IMemoryCache _inMemoryCache;
        private readonly ILogger<DashboardService> _logger;
        private const byte CachedTimeInMins = 1;                // Cache time in minutes


        public DashboardService(IUserServiceClient userClient, ITaskServiceClient taskClient, IJournalServiceClient journalClient, IMemoryCache cache, ILogger<DashboardService> logger)
        {
            _userClient = userClient;
            _taskClient = taskClient;
            _journalClient = journalClient;
            _inMemoryCache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Legacy
        /// Composes dashboard data from multiple services.
        /// Uses in-memory caching to avoid redundant downstream API calls.
        /// </summary>
        public async Task<DashboardDto> GetDashboardAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                _logger.LogWarning("Invalid user ID format received: {UserId}", userId);
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }

            var cacheKey = $"dashboard:{userGuid}";
            try
            {
                _logger.LogInformation("Attempting to retrieve dashboard for user {UserId} from cache", userGuid);

                // Check in-memory cache first
                if (_inMemoryCache.TryGetValue(cacheKey, out DashboardDto? cachedDashboard) && cachedDashboard is not null)
                {
                    _logger.LogInformation("Dashboard cache hit for user {UserId}", userGuid);
                    return cachedDashboard;
                }

                _logger.LogInformation("Dashboard cache miss/unavailable for user {UserId}. Fetching from downstream services...", userGuid);


                // No cache found - so call APIs
                // Fetch from downstream services
                var user = await _userClient.GetUserAsync(userId, cancellationToken);
                var tasks = await _taskClient.GetTasksAsync(userId, cancellationToken) ?? [];

                var dashboard = new DashboardDto(user, tasks);

                // Cache the composed result for n minutes
                _inMemoryCache.Set(cacheKey, dashboard, TimeSpan.FromMinutes(CachedTimeInMins));
                _logger.LogInformation("Dashboard cached for user {UserId} with expiration of {CacheDuration} minutes", userGuid, CachedTimeInMins);


                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve dashboard for user {UserId}", userGuid);
                throw new ApplicationException("Dashboard retrieval failed", ex);
            }
        }

        /// <summary>
        /// Returns a hydrated dto of Dashboard for the userId based on user role
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken for cancelling</param>
        /// <returns>BaseDashboardDto subtype</returns>
        public async Task<BaseDashboardDto> GetUserDashboardAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                _logger.LogWarning("Invalid user ID format received: {UserId}", userId);
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }
            try
            {
                _logger.LogInformation("Attempting to retrieve dashboard for user {UserId} from cache", userGuid);
                var user = await _userClient.GetUserAsync(userId, cancellationToken);

                if (user is null)
                {
                    _logger.LogError("No user with Id {UserId}. Unable to construct a dashboard result.", userGuid);
                    throw new ArgumentException($"Dashboard retrieval failed as there is no user with Id {userGuid}");
                }

                // return appropriate sub type of BaseDashboardDto based on user role
                return ConstructuserDashboard(userGuid, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve dashboard for user {UserId}", userGuid);
                throw new ApplicationException("Dashboard retrieval failed", ex);
            }
        }

        private BaseDashboardDto ConstructuserDashboard(Guid userGuid, UserAccountDto user)
        {
            switch (user.UserRole.ToLower())
            {
                case "admin":
                    {
                        _logger.LogInformation("Constructing Admin dashboard for {UserId}", userGuid);

                        // To Do: more admin specific dashboard data
                        // ..

                        return new AdminDashboardDto(user);
                    }
                case "user":
                    {
                        _logger.LogInformation("Constructing User dashboard for {UserId}", userGuid);

                        // To Do: more user (developer) specific dashboard data
                        // ..

                        return new UserDashboardDto(user);
                    }
                case "manager":
                    {
                        _logger.LogInformation("Constructing Manager dashboard for {UserId}", userGuid);

                        // To Do: more manager specific dashboard data
                        // ..


                        return new OrchestratorService.Application.DTOs.ManagerDashboardDto(user);
                    }
                default:
                    throw new ApplicationException($"Invalid user role for user {userGuid}");
            }
        }


        /// <summary>
        /// Invalidates the dashboard cache for a specific user if it exists.
        /// Ensures the 
        public void InvalidateDashboardCache(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                _logger.LogWarning("Invalid user ID format received: {UserId}", userId);
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }

            var cacheKey = $"dashboard:{userGuid}";

            try
            {
                _logger.LogInformation("Attempting to invalidate dashboard cache for user {UserId}", userGuid);

                // Check in-memory cache first
                if (!_inMemoryCache.TryGetValue(cacheKey, out DashboardDto? cachedDashboard) || cachedDashboard is null)
                {
                    _logger.LogInformation("No dashboard cache found for user {UserId}. Nothing to invalidate.", userGuid);
                    return;         // no cache to remove
                }

                _inMemoryCache.Remove(cacheKey);
                _logger.LogInformation("Dashboard cache successfully invalidated for user {UserId}", userGuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while invalidating dashboard cache for user {UserId}", userGuid);
                throw new ApplicationException("Dashboard cache invalidation failed", ex);
            }

        }

        /// <summary>
        /// Builds the developer dashboard by aggregating data from Tasks, Journals, Feedback,
        /// and User APIs. Results are cached per user to improve performance.
        /// </summary>
        /// <param name="userId">The unique identifier of the developer whose dashboard is requested.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the operation to complete.</param>
        /// <returns>
        /// A <see cref="DeveloperDashboardDto"/> containing summary cards, chart data,
        /// feedback counts, and user display information.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Attempts to retrieve the dashboard from in-memory cache.
        /// 2. If not found, queries downstream services to build the dashboard.
        /// 3. Caches the result for subsequent requests.
        /// </remarks>
        public async Task<DeveloperDashboardDto> GetDeveloperDashboardAsync(Guid userId, CancellationToken cancellationToken)
        {
            var cacheKey = $"DeveloperDashboardForUserId:{userId}";

            _logger.LogInformation("Developer dashboard request started for user {UserId}", userId);

            try
            {
                // STEP 0 — Try cache
                if (_inMemoryCache.TryGetValue(cacheKey, out DeveloperDashboardDto? cached) && cached is not null)
                {
                    _logger.LogInformation("Cache hit for user {UserId}", userId);
                    return cached;
                }

                _logger.LogInformation("Cache miss for user {UserId}", userId);

                // STEP 1 — Fetch data from downstream APIs
                _logger.LogInformation("Fetching tasks for user {UserId}", userId);
                var tasks = await _taskClient.GetTasksAsync(userId.ToString(), cancellationToken);

                _logger.LogInformation("Fetching journals with feedback for user {UserId}", userId);
                var journalsWithFeedback = await _journalClient.GetJournalsWithFeedback(userId, cancellationToken);

                _logger.LogInformation("Fetching user profile for user {UserId}", userId);
                var user = await _userClient.GetUserAsync(userId.ToString(), cancellationToken);

                // STEP 2 — Build DTO
                _logger.LogInformation("Building dashboard DTO for user {UserId}", userId);

                var dto = new DeveloperDashboardDto(
                    SummaryCardsDto: BuildSummary(
                        tasks,
                        journalsWithFeedback.Select(x => x.feedback).OfType<JournalFeedbackDto>()
                    ),
                    JournalsOverTimeLineChartDto: BuildJournalTimeSeries(
                        journalsWithFeedback.Select(x => x.journal)
                    ),
                    TaskStatusesDonutChartDto: BuildTaskStatuses(tasks),
                    JournalFeedbackCountsBarChartDto: BuildFeedbackCounts(journalsWithFeedback),
                    LastUpdated: TimeHelper.GetAusSydTime(),
                    UserId: userId,
                    UserDisplayName: user.DisplayName
                );

                // STEP 3 — Cache result
                _logger.LogInformation("Caching dashboard for user {UserId} for {Minutes} minutes",
                    userId, CachedTimeInMins);

                _inMemoryCache.Set(cacheKey, dto, TimeSpan.FromMinutes(CachedTimeInMins));

                _logger.LogInformation("Developer dashboard successfully built for user {UserId}", userId);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build developer dashboard for user {UserId}", userId);
                throw; // preserve stack trace
            }
        }

        private JournalFeedbackCountsDto BuildFeedbackCounts(IReadOnlyList<JournalEntryWithFeedbackDto> journals)
        {
            return new JournalFeedbackCountsDto(
                WithFeedBackJournalCount: journals.Count(x => x.feedback is not null),
                WithoutFeedBackJournalCount: journals.Count(x => x.feedback is null),
                TotalJounralCount: journals.Count
            );
        }

        private TaskStatusesCountsDto BuildTaskStatuses(IReadOnlyList<TaskItemDto> tasks)
        {
            return new TaskStatusesCountsDto(
                NotStartedTaskCount: tasks.Count(x => x.Status.Equals("NotStarted", StringComparison.OrdinalIgnoreCase)),
                InProgressTaskCount: tasks.Count(x => x.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
                CompletedTaskCount: tasks.Count(x => x.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            );
        }

        private List<TimeSeriesPointDto> BuildJournalTimeSeries(IEnumerable<JournalEntryDto> journals)
        {
            //return [.. journals
            //    .GroupBy(j => new { j.CreatedAt.Year, j.CreatedAt.Month })
            //    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            //    .Select(g =>
            //    {
            //        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month);
            //        string label = $"{g.Key.Year} - {monthName}";
            //        return new TimeSeriesPointDto(label, g.Count());
            //    })];

            // 1. Build a lookup of counts per (Year, Month)
            var journalCounts = journals
                .GroupBy(j => new { j.CreatedAt.Year, j.CreatedAt.Month })
                .ToDictionary(
                    g => (g.Key.Year, g.Key.Month),
                    g => g.Count()
                );

            // 2. Generate the last 12 months (including current month)
            var results = new List<TimeSeriesPointDto>();
            var now = DateTime.UtcNow;

            for (int i = 11; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                int year = date.Year;
                int month = date.Month;

                // 3. Get count or default to 0
                int count = journalCounts.TryGetValue((year, month), out int c) ? c : 0;

                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
                string label = $"{year} - {monthName}";

                results.Add(new TimeSeriesPointDto(label, count));
            }

            return results;
        }


        private SummaryCardsDto BuildSummary(IReadOnlyList<TaskItemDto> tasks, IEnumerable<JournalFeedbackDto> feedbacks)
        {
            return new SummaryCardsDto(
                HighPriorityCount: tasks.Count(x => x.Priority.Equals("high", StringComparison.OrdinalIgnoreCase)),
                NewTasksCount: tasks.Count(x => x.Status.Equals("NotStarted", StringComparison.OrdinalIgnoreCase)),
                InProgressTasksCount: tasks.Count(x => x.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
                UrgentTasksCount: tasks.Count(x => !x.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) 
                                                        && x.DueDate != null && (x.DueDate - DateTime.Today) <= TimeSpan.FromDays(7)),
                NewFeedbacksCount: feedbacks.Count(x => !x.SeenByUser)
            );
        }

        public async Task<ManagerDashboardDto> GetManagerDashboardAsync(Guid managerId, CancellationToken cancellationToken)
        {
            var cacheKey = $"ManagerDashboardForUserId:{managerId}";

            _logger.LogInformation("Manager dashboard request started for user {ManagerId}", managerId);

            try
            {
                // STEP 0 — Try cache
                if (_inMemoryCache.TryGetValue(cacheKey, out ManagerDashboardDto? cached) && cached is not null)
                {
                    _logger.LogInformation("Cache hit for user {ManagerId}", managerId);
                    return cached;
                }

                _logger.LogInformation("Cache miss for user {ManagerId}", managerId);

                // STEP 1 — Fetch data from downstream APIs
                _logger.LogInformation("Fetching tasks for user {ManagerId}", managerId);
                IReadOnlyList<UserAccountDto> teamMembers = await _userClient.GetTeamMembersForManager(managerId, cancellationToken);

                IReadOnlyList<Guid> teamMemberIds = [.. teamMembers.Select(x => x.Id)];
                IReadOnlyList<TaskItemDto> tasks = await _taskClient.GetTasksForTeamMembers(teamMemberIds, false, cancellationToken);

                _logger.LogInformation("Fetching journals with feedback for manager {ManagerId}", managerId);                
                IReadOnlyList<JournalEntryWithFeedbackDto> journalsWithFeedbacks = await _journalClient.GetJournalsForTeamFeedback(teamMemberIds, cancellationToken);

                _logger.LogInformation("Fetching user profile for manager {ManagerId}", managerId);
                var user = await _userClient.GetUserAsync(managerId.ToString(), cancellationToken);

                // STEP 2 — Build DTO
                _logger.LogInformation("Building dashboard DTO for manager {ManagerId}", managerId);                
                var dto = new ManagerDashboardDto(
                    SummaryCardsDto: BuildManagerSummary(
                        tasks,
                        journalsWithFeedbacks.Select(x => x.journal).OfType<JournalEntryDto>()
                    ),

                    TeamJournalsPerDeveloperBarChartDto: BuidTeamJounalsCounts(journalsWithFeedbacks.Select(x => x.journal).OfType<JournalEntryDto>(), teamMembers),
                    FeedbackDonutChartDto: BuildFeedbackFeedbackDonutChart(journalsWithFeedbacks),
                    TasksWithStatus: BuildTasksWithStatus(tasks),

                    LastUpdated: TimeHelper.GetAusSydTime(),
                    ManagerId: managerId,
                    ManagerDisplayName: user.DisplayName
                );

                // STEP 3 — Cache result
                _logger.LogInformation("Caching dashboard for manager {ManagerId} for {Minutes} minutes",
                    managerId, CachedTimeInMins);

                _inMemoryCache.Set(cacheKey, dto, TimeSpan.FromMinutes(CachedTimeInMins));

                _logger.LogInformation("Manager dashboard successfully built for manager {ManagerId}", managerId);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build manager dashboard for manager {ManagerId}", managerId);
                throw; // preserve stack trace
            }
        }

        private List<LabelNumberDto> BuildTasksWithStatus(IReadOnlyList<TaskItemDto> tasks)
        {
            return [.. from t in tasks 
                       group t by t.Status into statusGroup
                       select new LabelNumberDto(statusGroup.Key, statusGroup.Count())];
        }

        private FeedbackDonutChartDto BuildFeedbackFeedbackDonutChart(IReadOnlyList<JournalEntryWithFeedbackDto> journalsWithFeedbacks)
        {
            var feedbackCompleted = journalsWithFeedbacks.Count(x => x.journal.IsFeedbackGiven);
            return new FeedbackDonutChartDto(
                    FeedbackCompleted: feedbackCompleted,
                    FeedbackPending: journalsWithFeedbacks.Select(x => x.journal).Count() - feedbackCompleted
                );
        }

        private List<LabelNumberDto> BuidTeamJounalsCounts(IEnumerable<JournalEntryDto> journals, IReadOnlyList<UserAccountDto> teamMembers)
        {
            return [..from journal in journals
                   group journal by journal.UserId into userGroup
                   let userDisplayName = teamMembers.FirstOrDefault(x => x.Id == userGroup.Key)?.DisplayName ?? "-"
                   select new LabelNumberDto(userDisplayName, userGroup.Count())];
        }

        private ManagerSummaryCardsDto BuildManagerSummary(IReadOnlyList<TaskItemDto> tasks, IEnumerable<JournalEntryDto> journals)
        {
            return new ManagerSummaryCardsDto(
                HighPriorityCount: tasks.Count(x => x.Priority.Equals("high", StringComparison.OrdinalIgnoreCase)),
                NewTasksCount: tasks.Count(x => x.Status.Equals("NotStarted", StringComparison.OrdinalIgnoreCase)),
                InProgressTasksCount: tasks.Count(x => x.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
                UrgentTasksCount: tasks.Count(x => !x.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)
                                                        && x.DueDate != null && (x.DueDate - DateTime.Today) <= TimeSpan.FromDays(7)),
                NewJournalsNeedingFeedback: journals.Count(x => !x.IsFeedbackGiven)
            );
        }

    }
}
