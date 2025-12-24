using FluentValidation;
using JournalService.Application.Commands.Journal;

namespace JournalService.Application.Validators.Journal
{
    public class AddJournalEntryCommandValidator : AbstractValidator<AddJournalEntryCommand>
    {
        public AddJournalEntryCommandValidator()
        {
            RuleFor(x => x.UserId)
               .NotEmpty()
               .WithMessage("User Id must not be empty.");

            RuleFor(x => x.Title)
               .NotEmpty()
               .WithMessage("Title must not be empty.");

            RuleFor(x => x.Content)
               .NotEmpty()
               .WithMessage("Content must not be empty.");
        }
    }
}
