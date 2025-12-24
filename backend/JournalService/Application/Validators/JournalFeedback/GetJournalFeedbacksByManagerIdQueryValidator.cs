using FluentValidation;
using JournalService.Application.Queries.JournalFeedback;

namespace JournalService.Application.Validators.JournalFeedback
{
    public class GetJournalFeedbacksByManagerIdQueryValidator : AbstractValidator<GetJournalFeedbacksByManagerIdQuery>
    {
        public GetJournalFeedbacksByManagerIdQueryValidator()
        {
            RuleFor(x => x.ManagerId)
               .NotEmpty()
               .WithMessage("Manager Id must not be empty.");
        }
    }
}
