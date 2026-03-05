using FluentValidation;
using UserService.Application.Queries;

namespace UserService.Application.Validators
{
    public class GetTeamMemberGuidsForManagerQueryValidator : AbstractValidator<GetTeamMemberForManagerQuery>
    {
        public GetTeamMemberGuidsForManagerQueryValidator()
        {
            RuleFor(x => x.ManagerId)
                .NotEmpty().WithMessage("Manager Id must not be empty.");
        }
    }
}
