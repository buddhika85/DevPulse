using FluentValidation;
using JournalService.Application.Commands.Journal;

namespace JournalService.Application.Validators.Journal
{
    public class RestoreJournalEntryCommandValidator : AbstractValidator<RestoreJournalEntryCommand>
    {
        public RestoreJournalEntryCommandValidator()
        {
            RuleFor(x => x.JournalEntryId)
               .NotEmpty()
               .WithMessage("Journal Entry Id must not be empty.");
        }
    }
}
