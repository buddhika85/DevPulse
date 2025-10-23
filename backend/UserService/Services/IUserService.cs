using UserService.Application.Commands;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<Guid?> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken);
    }
}
