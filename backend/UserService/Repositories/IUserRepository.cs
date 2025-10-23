using SharedLib.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Repositories
{
    public interface IUserRepository : IBaseRepository<UserAccount>
    {
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
        //Task<PaginatedResult<TaskItem>> GetUserAccountsPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken);
    }
}
