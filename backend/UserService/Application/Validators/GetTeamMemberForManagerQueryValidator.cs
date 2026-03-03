using FluentValidation;
using UserService.Application.Queries;

namespace UserService.Application.Validators
{
    public class GetTeamMemberForManagerQueryValidator : AbstractValidator<GetTeamMemberForManagerQuery>
    {
        public GetTeamMemberForManagerQueryValidator()
        {
            RuleFor(x => x.ManagerId)
                .NotEmpty().WithMessage("Manager Id must not be empty.");
        }
    }
}
