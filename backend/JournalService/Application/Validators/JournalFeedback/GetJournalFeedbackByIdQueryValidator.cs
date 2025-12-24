using FluentValidation;
using JournalService.Application.Queries.JournalFeedback;

namespace JournalService.Application.Validators.JournalFeedback
{
    public class GetJournalFeedbackByIdQueryValidator : AbstractValidator<GetJournalFeedbackByIdQuery>
    {
        public GetJournalFeedbackByIdQueryValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .WithMessage("Jounral Feedback Id must not be empty.");        
        }
    }
}
