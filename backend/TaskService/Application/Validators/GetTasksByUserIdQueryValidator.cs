using FluentValidation;
using TaskService.Application.Queries;

namespace TaskService.Application.Validators
{
    public class GetTasksByUserIdQueryValidator : AbstractValidator<GetTasksByUserIdQuery>
    {
        public GetTasksByUserIdQueryValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty();
        }
    }
}
