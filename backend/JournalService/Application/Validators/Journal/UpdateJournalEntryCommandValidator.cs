using FluentValidation;
using JournalService.Application.Commands.Journal;

namespace JournalService.Application.Validators.Journal
{
    public class UpdateJournalEntryCommandValidator : AbstractValidator<UpdateJournalEntryCommand>
    {
        public UpdateJournalEntryCommandValidator()
        {
            RuleFor(x => x.JournalEntryId)
               .NotEmpty()
               .WithMessage("Journal Entry Id must not be empty.");

            RuleFor(x => x.Title)
               .NotEmpty()
               .WithMessage("Title must not be empty.");

            RuleFor(x => x.Content)
               .NotEmpty()
               .WithMessage("Content must not be empty.");
        }
    }
}
