using Microsoft.EntityFrameworkCore;
using TaskService.Application.Common.Enums;
using TaskService.Application.Common.Models;
using TaskService.Application.Queries;
using TaskService.Domain.Entities;
using TaskService.Infrastructure.Persistence;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TaskService.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext _dbContext;

        public TaskRepository(TaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(TaskItem task)
        {
            _dbContext.Tasks.Add(task);
        }

        public void Update(TaskItem task)
        {
            _dbContext.Tasks.Update(task);
        }

        public void Delete(TaskItem task)
        {
            _dbContext.Tasks.Remove(task);
        }


        public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellation)
        {
            return await _dbContext.Tasks.AsNoTracking().ToListAsync(cancellation);
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<PaginatedResult<TaskItem>> GetTasksPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken)
        {
            var queryable = _dbContext.Tasks.AsQueryable();

            if (query.TaskId.HasValue)
                queryable = queryable.Where(x => x.Id == query.TaskId.Value);
            if (!string.IsNullOrWhiteSpace(query.Title))
                queryable = queryable.Where(x => !string.IsNullOrWhiteSpace(x.Title) && EF.Functions.Like(x.Title, $"%{query.Title}%"));
            if (!string.IsNullOrWhiteSpace(query.Description))
                queryable = queryable.Where(x => !string.IsNullOrWhiteSpace(x.Description) && EF.Functions.Like(x.Description, $"%{query.Description}%"));
            if (query.IsCompleted.HasValue)
                queryable = queryable.Where(x => x.IsCompleted == query.IsCompleted.Value);
           
            if (query.SortBy is not null)
            {
                queryable = query.SortBy switch
                {
                    TaskSortField.Title => query.SortDescending ? queryable.OrderByDescending(x => x.Title) : queryable.OrderBy(x => x.Title),
                    TaskSortField.Description => query.SortDescending ? queryable.OrderByDescending(x => x.Description) : queryable.OrderBy(x => x.Description),
                    TaskSortField.IsCompleted => query.SortDescending ? queryable.OrderByDescending(x => x.IsCompleted) : queryable.OrderBy(x => x.IsCompleted),
                    _ => queryable.OrderBy(x => x.CreatedAt),
                };
            }
            else
            {
                queryable = queryable.OrderBy(x => x.CreatedAt);
            }


            var totalCount = await queryable.CountAsync(cancellationToken);

            // execute queryable
            var pagedData = await queryable.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(cancellationToken);

            return new PaginatedResult<TaskItem> 
            { 
                PageItems = pagedData,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };            
        }
    }
}
