using MediatR;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Commands
{
    public record UpdateUserCommand(Guid Id, string Email, string DisplayName, UserRole? Role) : IRequest<bool>
    {
    }
}
