using FluentValidation;
using JournalService.Application.Queries.Journal;

namespace JournalService.Application.Validators.Journal
{
    public class GetJournalEntriesByUserIdQueryValidator : AbstractValidator<GetJournalEntriesByUserIdQuery>
    {
        public GetJournalEntriesByUserIdQueryValidator()
        {
            RuleFor(x => x.UserId)
               .NotEmpty()
               .WithMessage("User Id must not be empty.");
        }
    }
}
