using Microsoft.EntityFrameworkCore;
using TaskService.Application.Common.Enums;
using TaskService.Application.Common.Models;
using TaskService.Application.Queries;
using TaskService.Domain.Entities;
using TaskService.Infrastructure.Persistence;

namespace TaskService.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext _dbContext;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(TaskDbContext dbContext, ILogger<TaskRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<TaskItem?> AddAsync(TaskItem entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add new Task with title: {Title} at {Time}", entity.Title, DateTime.UtcNow);
            try
            {
                await _dbContext.Tasks.AddAsync(entity, cancellationToken);
                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully added Task with title: {Title} at {Time}", entity.Title, DateTime.UtcNow);
                    return entity;
                }

                _logger.LogWarning("No records added for Task with title: {Title} at {Time}", entity.Title, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding Task with title: {Title} at {Time}", entity.Title, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete Task with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No Task found with Id: {Id} at {Time}. Nothing was deleted.", id, DateTime.UtcNow);
                    return false;
                }

                _dbContext.Tasks.Remove(entity);
                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully deleted Task titled '{Title}' with Id: {Id} at {Time}", entity.Title, id, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Delete operation for Task titled '{Title}' with Id: {Id} at {Time} did not affect any records.", entity.Title, id, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting Task with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve all TaskItems at {Time}", DateTime.UtcNow);
            try
            {
                var entities = await _dbContext.Tasks
                    .AsNoTracking()
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} TaskItems at {Time}", entities.Count, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all TaskItems at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve Task with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No Task found with Id: {Id} at {Time}", id, DateTime.UtcNow);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved Task titled '{Title}' with Id: {Id} at {Time}", entity.Title, id, DateTime.UtcNow);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving Task with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }        
             
        public async Task<bool> UpdateAsync(Guid id, TaskItem entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update Task with Id: {Id}, Title: {Title} at {Time}", id, entity.Title, DateTime.UtcNow);

            try
            {
                var existing = await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
                if (existing is null)
                {
                    _logger.LogWarning("No Task found with Id: {Id} at {Time}. Update aborted.", id, DateTime.UtcNow);
                    return false;
                }

                // Option 1: Update only allowed fields
                existing.Title = entity.Title;
                existing.Description = entity.Description;
                existing.IsCompleted = entity.IsCompleted;

                // Option 2 (if we are trusting the incoming object): 
                // _dbContext.Entry(existing).CurrentValues.SetValues(entity);

                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully updated Task with Id: {Id}, Title: {Title} at {Time}", id, entity.Title, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Update operation for Task with Id: {Id}, Title: {Title} at {Time} did not affect any records.", id, entity.Title, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating Task with Id: {Id}, Title: {Title} at {Time}", id, entity.Title, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<PaginatedResult<TaskItem>> GetTasksPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Attempting to retrieve paginated TaskItems with filters: TaskId={TaskId}, Title={Title}, Description={Description}, IsCompleted={IsCompleted}, Page={PageNumber}, Size={PageSize}, SortBy={SortBy}, Desc={SortDescending} at {Time}",
                query.TaskId, query.Title, query.Description, query.IsCompleted, query.PageNumber, query.PageSize, query.SortBy, query.SortDescending, DateTime.UtcNow);

            try
            {
                var queryable = _dbContext.Tasks.AsNoTracking().AsQueryable();

                if (query.TaskId.HasValue)
                    queryable = queryable.Where(x => x.Id == query.TaskId.Value);

                if (!string.IsNullOrWhiteSpace(query.Title))
                    queryable = queryable.Where(x => EF.Functions.Like(x.Title, $"%{query.Title}%"));

                if (!string.IsNullOrWhiteSpace(query.Description))
                    queryable = queryable.Where(x => EF.Functions.Like(x.Description, $"%{query.Description}%"));

                if (query.IsCompleted.HasValue)
                    queryable = queryable.Where(x => x.IsCompleted == query.IsCompleted.Value);

                queryable = query.SortBy switch
                {
                    TaskSortField.Title => query.SortDescending ? queryable.OrderByDescending(x => x.Title) : queryable.OrderBy(x => x.Title),
                    TaskSortField.Description => query.SortDescending ? queryable.OrderByDescending(x => x.Description) : queryable.OrderBy(x => x.Description),
                    TaskSortField.IsCompleted => query.SortDescending ? queryable.OrderByDescending(x => x.IsCompleted) : queryable.OrderBy(x => x.IsCompleted),
                    _ => queryable.OrderBy(x => x.CreatedAt),
                };

                var totalCount = await queryable.CountAsync(cancellationToken);

                var pagedData = await queryable
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} TaskItems for page {PageNumber} at {Time}", pagedData.Count, query.PageNumber, DateTime.UtcNow);

                return new PaginatedResult<TaskItem>
                {
                    PageItems = pagedData,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving paginated TaskItems at {Time}", DateTime.UtcNow);
                throw;
            }
        }
    }
}
