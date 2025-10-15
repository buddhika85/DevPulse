using TaskService.Models;

namespace TaskService.Application.Repositories
{
    public interface ITaskRepository
    {
        void Add(TaskItem task);
        void Update(TaskItem task);
        void Delete(TaskItem task);


        // every method that touches the database or performs I/O should accept a CancellationToken.
        // This allows graceful shutdown, client disconnect handling, and better resource management.
        Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken);
        Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);           
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
