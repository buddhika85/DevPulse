using FluentValidation;
using TaskService.Application.Queries;

namespace TaskService.Application.Validators
{
    public class GetTasksByTeamQueryValidator : AbstractValidator<GetTasksByTeamQuery>
    {
        public GetTasksByTeamQueryValidator()
        {
            RuleFor(x => x.TeamMembers)
                .NotEmpty();
        }
    }
}
