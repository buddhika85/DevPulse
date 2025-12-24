using FluentValidation;
using JournalService.Application.Commands.JournalFeedback;

namespace JournalService.Application.Validators.JournalFeedback
{
    public class AddJournalFeedbackCommandValidator : AbstractValidator<AddJournalFeedbackCommand>
    {
        public AddJournalFeedbackCommandValidator()
        {
            RuleFor(x => x.JounralEntryId)
               .NotEmpty()
               .WithMessage("Jounral Entry Id must not be empty.");

            RuleFor(x => x.FeedbackManagerId)
               .NotEmpty()
               .WithMessage("Feedback Manager Id must not be empty.");

            RuleFor(x => x.Comment)
               .NotEmpty()
               .WithMessage("Comment must not be empty.");
        }
    }
}
