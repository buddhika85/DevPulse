using FluentValidation;
using TaskService.Application.Queries;

namespace TaskService.Application.Validators
{
    public class GetTasksByIdsQueryValidator : AbstractValidator<GetTasksByIdsQuery>
    {
        public GetTasksByIdsQueryValidator()
        {
            RuleFor(x => x.taskIds)
                .NotEmpty();
        }
    }
}
