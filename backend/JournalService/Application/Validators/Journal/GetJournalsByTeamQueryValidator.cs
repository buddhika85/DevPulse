using FluentValidation;
using JournalService.Application.Queries.Journal;

namespace JournalService.Application.Validators.Journal
{
    public class GetJournalsByTeamQueryValidator : AbstractValidator<GetJournalsByTeamQuery>
    {
        public GetJournalsByTeamQueryValidator()
        {
            RuleFor(x => x.TeamMemberIds)
               .NotEmpty()
               .WithMessage("Team Member Ids must not be empty.");
        }
    }
}
