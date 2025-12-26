using FluentValidation;
using JournalService.Application.Queries.Journal;

namespace JournalService.Application.Validators.Journal
{
    public class IsJournalEntryExistsByIdQueryValidator : AbstractValidator<IsJournalEntryExistsByIdQuery>
    {
        public IsJournalEntryExistsByIdQueryValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .WithMessage("Journal Id must not be empty.");
        }
    }
}
