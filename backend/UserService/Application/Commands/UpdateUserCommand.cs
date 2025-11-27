using MediatR;
using SharedLib.Domain.ValueObjects;

namespace UserService.Application.Commands
{
    public record UpdateUserCommand(Guid Id, string? Email, string? DisplayName, UserRole? Role, Guid? ManagerId) : IRequest<bool>
    {
    }
}
