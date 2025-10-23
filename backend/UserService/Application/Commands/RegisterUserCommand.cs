using MediatR;

namespace UserService.Application.Commands
{
    // Fluent Validator for this CreateTaskCommandValidator
    public class RegisterUserCommand : IRequest<Guid?>
    {
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = "user";
    }
}
