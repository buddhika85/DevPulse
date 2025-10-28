using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators
{
    public class RestoreUserCommandValidator : AbstractValidator<RestoreUserCommand> 
    {
        public RestoreUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id must not be empty.");
        }
    }
}
