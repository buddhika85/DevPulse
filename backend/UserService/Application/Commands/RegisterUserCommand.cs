using MediatR;

namespace UserService.Application.Commands
{
    // Fluent Validator for this RegisterUserCommandValidator
    public record RegisterUserCommand(string Email, string DisplayName, DateTime CreatedAt, string Role = "user") : IRequest<Guid?>
    {
    }
}
