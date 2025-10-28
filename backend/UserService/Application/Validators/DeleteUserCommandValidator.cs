using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id must not be empty.");
        }
    }
}
