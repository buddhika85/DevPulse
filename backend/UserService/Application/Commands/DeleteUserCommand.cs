using MediatR;

namespace UserService.Application.Commands
{
    public record DeleteUserCommand(Guid Id) : IRequest<bool>
    { }
}
