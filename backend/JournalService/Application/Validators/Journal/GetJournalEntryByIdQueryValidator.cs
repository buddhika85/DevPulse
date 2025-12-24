using FluentValidation;
using JournalService.Application.Queries.Journal;

namespace JournalService.Application.Validators.Journal
{
    public class GetJournalEntryByIdQueryValidator : AbstractValidator<GetJournalEntryByIdQuery>
    {
        public GetJournalEntryByIdQueryValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .WithMessage("Journal Id must not be empty.");
        }
    }
}
