using UserService.Application.Commands;
using UserService.Application.Dtos;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserAccountDto>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<Guid?> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken);
    }
}
