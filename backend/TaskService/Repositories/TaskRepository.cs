using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.Common.Enums;
using TaskService.Application.Common.Models;
using TaskService.Application.Queries;
using TaskService.Domain.Entities;
using TaskService.Infrastructure.Common.Extensions;
using TaskService.Infrastructure.Persistence;

namespace TaskService.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext _dbContext;
        private readonly ILogger<TaskRepository> _logger;
        private readonly IMediator _mediator;

        public TaskRepository(TaskDbContext dbContext, ILogger<TaskRepository> logger, IMediator mediator)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<TaskItem?> AddAsync(TaskItem entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add new Task with title: {Title} at {Time}", entity.Title, DateTime.UtcNow);
            try
            {
                await _dbContext.Tasks.AddAsync(entity, cancellationToken);
                var result = await SaveChangesAsync(cancellationToken);

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

                // _dbContext.Tasks.Remove(entity);             // this permenently deletes from DB
                // entity.RaiseDeletedEvent();

                entity.SoftDelete();
               
                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully deleted Task titled '{Title}' with Id: {Id} at {Time}", entity.Title, id, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Delete operation for Task titled '{Title}' with Id: {Id} at {Time} did not affect any records.", entity.Title, id, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting Task with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to restore Task with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No Task found with Id: {Id} at {Time}. Nothing was deleted.", id, DateTime.UtcNow);
                    return false;
                }

                
                entity.Restore();

                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully restored Task titled '{Title}' with Id: {Id} at {Time}", entity.Title, id, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Restore operation for Task titled '{Title}' with Id: {Id} at {Time} did not affect any records.", entity.Title, id, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while restoring Task with Id: {Id} at {Time}", id, DateTime.UtcNow);
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
                    .Where(x => !x.IsDeleted)
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

        public async Task<IReadOnlyList<TaskItem>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false)
        {
            _logger.LogInformation("Attempting to retrieve all TaskItems for UserId={UserId} includeDeleted={IncludeDeleted} at {Time}", userId, includeDeleted, DateTime.UtcNow);
            try
            {
                var query = _dbContext.Tasks
                    .AsNoTracking()
                    .Where(x => x.UserId == userId);

                if (!includeDeleted)
                    query = query.Where(x => !x.IsDeleted);

                 var entities = await query.OrderByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} TaskItems for UserId={UserId} includeDeleted={IncludeDeleted} at {Time}", entities.Count, userId, includeDeleted, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all TaskItems for UserId={UserId} includeDeleted={IncludeDeleted} at {Time}", userId, includeDeleted, DateTime.UtcNow);
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
             
        public async Task<bool> UpdateAsync(Guid id, TaskItem updated, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update Task with Id: {Id}, Title: {Title} at {Time}", id, updated.Title, DateTime.UtcNow);

            try
            {
                var existing = await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);

               
                if (existing is null)
                {
                    _logger.LogWarning("No Task found with Id: {Id} at {Time}. Update aborted.", id, DateTime.UtcNow);
                    return false;
                }

                // applies changes to Db record and and raises events
                ApplyChanges(existing, updated);

                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully updated Task with Id: {Id}, Title: {Title} at {Time}", id, updated.Title, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Update operation for Task with Id: {Id}, Title: {Title} at {Time} did not affect any records.", id, updated.Title, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating Task with Id: {Id}, Title: {Title} at {Time}", id, updated.Title, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<PaginatedResult<TaskItem>> GetTasksPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Attempting to retrieve paginated TaskItems with filters: TaskId={TaskId}, Title={Title}, Description={Description}, Status={Status}, Page={PageNumber}, Size={PageSize}, SortBy={SortBy}, Desc={SortDescending} at {Time}",
                query.TaskId, query.Title, query.Description, query.Status, query.PageNumber, query.PageSize, query.SortBy, query.SortDescending, DateTime.UtcNow);

            try
            {
                var queryable = _dbContext.Tasks.AsNoTracking().Where(x => !x.IsDeleted).AsQueryable();

                if (query.TaskId.HasValue)
                    queryable = queryable.Where(x => x.Id == query.TaskId.Value);

                if (!string.IsNullOrWhiteSpace(query.Title))
                    queryable = queryable.Where(x => EF.Functions.Like(x.Title, $"%{query.Title}%"));

                if (!string.IsNullOrWhiteSpace(query.Description))
                    queryable = queryable.Where(x => EF.Functions.Like(x.Description, $"%{query.Description}%"));

                if (!string.IsNullOrWhiteSpace(query.Status))
                {
                    try
                    {
                        var taskStatus = Domain.ValueObjects.TaskStatus.From(query.Status);
                        queryable = queryable.Where(x => x.TaskStatus == taskStatus);
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.LogWarning(ex, "Invalid TaskStatus filter value: {Status}", query.Status);
                    }
                }

                queryable = query.SortBy switch
                {
                    TaskSortField.Title => query.SortDescending ? queryable.OrderByDescending(x => x.Title) : queryable.OrderBy(x => x.Title),
                    TaskSortField.Description => query.SortDescending ? queryable.OrderByDescending(x => x.Description) : queryable.OrderBy(x => x.Description),
                    TaskSortField.Status => query.SortDescending ? queryable.OrderByDescending(x => x.TaskStatus) : queryable.OrderBy(x => x.TaskStatus),
                    _ => queryable.OrderBy(x => x.CreatedAt),
                };

                // commented at these sends 2 queries to DB
                //var totalCount = await queryable.CountAsync(cancellationToken);
                //var pagedData = await queryable
                //    .Skip((query.PageNumber - 1) * query.PageSize)
                //    .Take(query.PageSize)
                //    .ToListAsync(cancellationToken);

                // this is to avoid sending 2 DB calls for counting and getting paged results
                var projectedResult = await
                                        (from x in queryable
                                          group x by 1 into fakeGroup
                                          select new
                                          {
                                              TotalCount = fakeGroup.Count(),
                                              PageItems = fakeGroup.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList(),
                                          }).SingleAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} TaskItems for page {PageNumber} at {Time}", projectedResult.PageItems.Count, query.PageNumber, DateTime.UtcNow);

                return new PaginatedResult<TaskItem>
                {
                    PageItems = projectedResult.PageItems,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalCount = projectedResult.TotalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving paginated TaskItems at {Time}", DateTime.UtcNow);
                throw;
            }
        }


        private async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var countOfWrites = await _dbContext.SaveChangesAsync(cancellationToken);

            // Dispatches all domain events raised by tracked entities
            // Happens after the database commit, ensuring events only fire if persistence succeeds
            await _mediator.DispatchDomainEventsAsync(_dbContext);

            return countOfWrites;
        }


        // a helper method for updates, it applies changes and raises domain events as needed
        private void ApplyChanges(TaskItem existing, TaskItem incoming)
        {
            // raise completed event if needed
            if (existing.TaskStatus == Domain.ValueObjects.TaskStatus.Pending && incoming.TaskStatus == Domain.ValueObjects.TaskStatus.Completed)
                existing.MarkCompleted();
            // raise reopened event if needed
            if (existing.TaskStatus == Domain.ValueObjects.TaskStatus.Completed && incoming.TaskStatus == Domain.ValueObjects.TaskStatus.Pending)
                existing.Reopen();

            // raise updated event if its a true update with a change           
            if (existing.Title != incoming.Title || existing.Description != incoming.Description)
                existing.Update(incoming.Title, incoming.Description, incoming.TaskStatus, incoming.TaskPriority, incoming.DueDate);
        }        
    }
}
