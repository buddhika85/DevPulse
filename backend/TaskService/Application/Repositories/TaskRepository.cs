using Microsoft.EntityFrameworkCore;
using TaskService.Data;
using TaskService.Models;

namespace TaskService.Application.Repositories
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
    }
}
