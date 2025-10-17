using SharedLib.Domain.Entities;

namespace SharedLib.Application.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        // every method that touches the database or performs I/O should accept a CancellationToken.
        // This allows graceful shutdown, client disconnect handling, and better resource management.

        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
