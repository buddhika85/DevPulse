using MediatR;

namespace UserService.Application.Commands
{
    public record RestoreUserCommand(Guid Id) : IRequest<bool>
    { }


}
