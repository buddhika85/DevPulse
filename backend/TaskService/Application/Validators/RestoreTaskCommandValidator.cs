using FluentValidation;
using TaskService.Application.Commands;

namespace TaskService.Application.Validators
{
    public class RestoreTaskCommandValidator : AbstractValidator<RestoreTaskCommand>
    {
        public RestoreTaskCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
