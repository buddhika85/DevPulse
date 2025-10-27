using SharedLib.Application.Interfaces;
using UserService.Application.Common.Models;
using UserService.Application.Queries;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Repositories
{
    public interface IUserRepository : IBaseRepository<UserAccount>
    {
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
        Task<IReadOnlyList<UserAccount>> GetAllAsync(UserRole role, CancellationToken cancellationToken);
        Task<bool> RestoreUser(Guid id, CancellationToken cancellationToken);
        Task<PaginatedResult<UserAccount>> GetUserAccountsPaginatedAsync(GetUsersPaginatedQuery query, CancellationToken cancellationToken);

        // Filter by role or tenant (future)
        //GetByRoleIdAsync();
        //GetByTenantIdAsync();

    }
}
