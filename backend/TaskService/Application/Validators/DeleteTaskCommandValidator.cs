using FluentValidation;
using TaskService.Application.Commands;

namespace TaskService.Application.Validators
{
    public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
    {
        public DeleteTaskCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
