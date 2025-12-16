using SharedLib.Application.Models;
using SharedLib.DTOs.User;
using UserService.Application.Commands;
using UserService.Application.Queries;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserAccountDto>> GetAllUsersAsync(GetAllUsersQuery query, CancellationToken cancellationToken);
        Task<IReadOnlyList<UserAccountDto>> GetAllUsersByRoleAsync(GetAllUsersByRoleQuery query, CancellationToken cancellationToken);
        Task<PaginatedResult<UserAccountDto>> GetUserAccountsPaginatedAsync(GetUsersPaginatedQuery query, CancellationToken cancellationToken);
        Task<UserAccountDto?> GetUserByIdAsync(GetUserByIdQuery query, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(ExistsByEmailQuery query, CancellationToken cancellationToken);


        Task<Guid?> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken);
        Task<bool> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken);
        Task<bool> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellationToken);
        Task<bool> RestoreUserAsync(RestoreUserCommand command, CancellationToken cancellationToken);


        Task<UserAccountDto?> ResolveOrCreateAsync(string userId, CancellationToken cancellationToken);         // userId == entra tokens oid

    }
}
