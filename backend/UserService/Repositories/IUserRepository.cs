using SharedLib.Application.Interfaces;
using SharedLib.Domain.ValueObjects;
using UserService.Application.Common.Models;
using UserService.Application.Queries;
using UserService.Domain.Entities;

namespace UserService.Repositories
{
    public interface IUserRepository : IBaseRepository<UserAccount>
    {
        Task<IReadOnlyList<UserAccount>> GetAllAsync(bool includeDeleted, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
        Task<IReadOnlyList<UserAccount>> GetAllAsync(UserRole role, CancellationToken cancellationToken);
        
        Task<PaginatedResult<UserAccount>> GetUserAccountsPaginatedAsync(GetUsersPaginatedQuery query, CancellationToken cancellationToken);


        Task<bool> SoftDeleteUserAsync(UserAccount userToSoftDelete, CancellationToken cancellationToken);
        Task<bool> RestoreUserAsync(UserAccount userToRestore, CancellationToken cancellationToken);
        Task<bool> IsUserExistsAsync(Guid managerId, UserRole manager, CancellationToken cancellationToken);

        // Filter by role or tenant (future)
        //GetByRoleIdAsync();
        //GetByTenantIdAsync();

    }
}
