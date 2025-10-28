using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators
{
    public class RegisterUserCommandValidator  : AbstractValidator<RegisterUserCommand>
    {
        private static readonly string[] AllowedRoles = { "user", "manager", "admin" };

        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .Matches(@"^[\w\.-]+@([\w-]+\.)+(com|net|org|edu)$")                            // regex
                .WithMessage("Email must be a valid domain (e.g., .com, .net, .org)");            

            RuleFor(x => x.DisplayName)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(50);

            RuleFor(x => x.Role)            // manager, Manager, User, user, admin, Admin
                .NotEmpty()
                .Must(role => AllowedRoles.Contains(role.ToLower()))
                .WithMessage("Role must be one of: User, Manager, Admin.");
        }
    }
}
