using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private static readonly string[] AllowedRoles = { "user", "manager", "admin" };
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id must not be empty.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email));            // x.Email is not null && x.Email.Trim() is not ""

            RuleFor(x => x.Role)
                .Must(role => role is not null && AllowedRoles.Contains(role?.Value.ToLower()))
                .When(x => x.Role is not null);
        }
    }
}
